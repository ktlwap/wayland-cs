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
            
            _readBuffer.Write(buffer);
            return length;
        }
        
        return 0;
    }

    private void InternalWrite()
    {
        
    }

    private bool HasFirstPollFd(PollEvents @event)
    {
        return _pollfds[0].revents.HasFlag(@event);
    }

    public void Write(in Span<byte> data)
    {
        _writeBuffer.Write(in data);
    }
    
    public int Read(ref Span<byte> buffer)
    {
        return _readBuffer.Read(ref buffer);
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        Syscall.close(_socket);
    }
}
