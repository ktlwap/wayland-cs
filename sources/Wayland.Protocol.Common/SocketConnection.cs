using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

public sealed class SocketConnection : IDisposable
{
    private readonly NetworkStream _networkStream;
    
    public readonly Socket _socket;
    public readonly MessageReader MessageReader;
    public readonly MessageWriter MessageWriter;

    public SocketConnection(string unixSocketPath)
    {
        _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        _socket.Connect(new UnixDomainSocketEndPoint(unixSocketPath));
        _networkStream = new NetworkStream(_socket);
        MessageReader = new MessageReader(this, _networkStream);
        MessageWriter = new MessageWriter(this, _networkStream);
    }

    public void SendFileDescriptor(int fd)
    {
        int sockFd = (int)_socket.Handle;
        byte[] data = [0];
        
        UnixInterop.iovec iov = new UnixInterop.iovec
        {
            iov_base = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0),
            iov_len = data.Length
        };
        
        int fdSize = sizeof(int);
        int cmsgSpace = UnixInterop.CMsgSpace(fdSize);
        IntPtr controlBuffer = Marshal.AllocHGlobal(cmsgSpace);
        Span<byte> zero = new byte[cmsgSpace];
        Marshal.Copy(zero.ToArray(), 0, controlBuffer, cmsgSpace);
        
        UnixInterop.cmsghdr cmsg = new UnixInterop.cmsghdr
        {
            cmsg_len = UnixInterop.CMsgLength(fdSize),
            cmsg_level = UnixInterop.SOL_SOCKET,
            cmsg_type = UnixInterop.SCM_RIGHTS
        };
        Marshal.StructureToPtr(cmsg, controlBuffer, false);
        
        IntPtr dataPtr = controlBuffer + UnixInterop.CMsgAlign(Marshal.SizeOf<UnixInterop.cmsghdr>());
        Marshal.WriteInt32(dataPtr, fd);
        
        UnixInterop.msghdr msg = new UnixInterop.msghdr
        {
            msg_iov = Marshal.AllocHGlobal(Marshal.SizeOf<UnixInterop.iovec>()),
            msg_iovlen = 1,
            msg_control = controlBuffer,
            msg_controllen = cmsgSpace
        };
        Marshal.StructureToPtr(iov, msg.msg_iov, false);

        int ret = UnixInterop.sendmsg(sockFd, ref msg, 0);
        if (ret == -1)
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        Marshal.FreeHGlobal(controlBuffer);
        Marshal.FreeHGlobal(msg.msg_iov);
    }

    public int ReadFileDescriptor()
    {
        int sockFd = (int) _socket.Handle;
        byte[] data = new byte[1];

        UnixInterop.iovec iov = new UnixInterop.iovec
        {
            iov_base = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0),
            iov_len = data.Length
        };

        int fdSize = sizeof(int);
        int cmsgSpace = UnixInterop.CMsgSpace(fdSize);
        IntPtr controlBuffer = Marshal.AllocHGlobal(cmsgSpace);

        UnixInterop.msghdr msg = new UnixInterop.msghdr
        {
            msg_iov = Marshal.AllocHGlobal(Marshal.SizeOf<UnixInterop.iovec>()),
            msg_iovlen = 1,
            msg_control = controlBuffer,
            msg_controllen = cmsgSpace
        };
        Marshal.StructureToPtr(iov, msg.msg_iov, false);

        int ret = UnixInterop.recvmsg(sockFd, ref msg, 0);
        if (ret == -1)
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        IntPtr dataPtr = controlBuffer + UnixInterop.CMsgAlign(Marshal.SizeOf<UnixInterop.cmsghdr>());
        int receivedFd = Marshal.ReadInt32(dataPtr);

        Marshal.FreeHGlobal(controlBuffer);
        Marshal.FreeHGlobal(msg.msg_iov);

        return receivedFd;
    }

    public void Dispose()
    {
        _socket.Dispose();
        _networkStream.Dispose();
    }
}
