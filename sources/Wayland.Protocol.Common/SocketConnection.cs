using System.Runtime.InteropServices;
using Mono.Unix;
using Mono.Unix.Native;

namespace Wayland.Protocol.Common;

public sealed class SocketConnection : IDisposable
{
    private readonly int _socket;
    private readonly RingBuffer _readBuffer;
    private readonly RingBuffer _writeBuffer;
    private readonly Pollfd[] _pollfds;
    
    public bool IsDataAvailable => _readBuffer.AvailableBytesCount > 0;
    
    public SocketConnection(string path)
    {
        _socket = Syscall.socket(UnixAddressFamily.AF_UNIX, UnixSocketType.SOCK_STREAM, 0);
        if (_socket < 0)
            throw new IOException("Failed to create UNIX socket.");
        
        if (Syscall.connect(_socket, new SockaddrUn(path)) != 0)
            throw new IOException($"Failed to connect to UNIX socket. Reason: {Syscall.GetLastError()}");

        _readBuffer = new RingBuffer(2048 * 8);
        _writeBuffer = new RingBuffer(2048 * 8);
        
        _pollfds = new Pollfd[]
        {
            new Pollfd
            {
                events = PollEvents.POLLIN,
                fd = _socket
            }
        };
    }

    private long InternalRead(int timeout)
    {
        if (Syscall.poll(_pollfds, timeout) > 0 && !HasFirstPollFd(PollEvents.POLLHUP))
        {
            byte[] buffer = new byte[4096];
            byte[] cmsg = new byte[1024];
            Msghdr msghdr = new Msghdr
            {
                msg_control = cmsg,
                msg_controllen = cmsg.Length,
                msg_flags = MessageFlags.MSG_CMSG_CLOEXEC,
            };
            
            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            
            Iovec[] iovec = new Iovec[] {
                new Iovec {
                    iov_base = bufferHandle.AddrOfPinnedObject(),
                    iov_len = (ulong) buffer.Length,
                },
            };
            
            msghdr.msg_iov = iovec;
            msghdr.msg_iovlen = iovec.Length;
            
            long length = Syscall.recvmsg(_socket, msghdr, MessageFlags.MSG_DONTWAIT | MessageFlags.MSG_CMSG_CLOEXEC);
            
            bufferHandle.Free();

            if (length == 0)
                return 0;
            if (length < 0)
                UnixMarshal.ThrowExceptionForLastError();
            
            _readBuffer.Write(buffer, 0, (int) length);
            return length;
        }
        
        return 0;
    }

    private void InternalWrite()
    {
        lock (_writeBuffer)
        {
            byte[] cmsgbuffer = new byte[Syscall.CMSG_SPACE(0 * sizeof(int))];
            Cmsghdr cmsghdr = new Cmsghdr
            {
                cmsg_len = (long) Syscall.CMSG_LEN(0 * sizeof(int)),
                cmsg_level = UnixSocketProtocol.SOL_SOCKET,
                cmsg_type = UnixSocketControlMessage.SCM_RIGHTS,
            };
            
            Msghdr msghdr = new Msghdr
            {
                msg_control = cmsgbuffer,
                msg_controllen = cmsgbuffer.Length,
            };
            cmsghdr.WriteToBuffer(msghdr, 0);

            byte[] buffer = new byte[_writeBuffer.AvailableBytesCount];
            _writeBuffer.Read(buffer, 0, buffer.Length);
            
            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            
            Iovec[] iovs = new Iovec[] {
                new Iovec {
                    iov_base = bufferHandle.AddrOfPinnedObject(),
                    iov_len = (ulong)buffer.Length,
                },
            };
            msghdr.msg_iov = iovs;
            msghdr.msg_iovlen = 1;

            if (Syscall.sendmsg(_socket, msghdr, 0) < 0)
                UnixMarshal.ThrowExceptionForLastError();

            bufferHandle.Free();
        }
    }

    private bool HasFirstPollFd(PollEvents @event)
    {
        return _pollfds[0].revents.HasFlag(@event);
    }

    public void Write(byte[] data, int index = 0, int count = 0)
    {
        _writeBuffer.Write(data, count, index);
    }
    
    public int Read(byte[] data, int index = 0, int count = 0)
    {
        return _readBuffer.Read(data, index, count);
    }

    public void Flush(int timeout)
    {
        InternalRead(timeout);
        InternalWrite();
    }
    
    public void Dispose()
    {
        Syscall.close(_socket);
    }
}
