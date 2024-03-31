using Mono.Unix.Native;

namespace Wayland.Protocol.Common;

public sealed class SocketConnection : IDisposable
{
    private readonly int _socket;
    private readonly Pollfd[] _pollFds;
    
    public SocketConnection(string path)
    {
        _socket = Syscall.socket(UnixAddressFamily.AF_UNIX, UnixSocketType.SOCK_STREAM, 0);
        if (_socket < 0)
            throw new IOException("Failed to create UNIX socket.");
        
        if (Syscall.connect(_socket, new SockaddrUn(path)) != 0)
            throw new IOException($"Failed to connect to UNIX socket. Reason: {Syscall.GetLastError()}");
        
        _pollFds = new Pollfd[]
        {
            new Pollfd
            {
                events = PollEvents.POLLIN,
                fd = _socket
            }
        };
    }

    private int Read(int timeout)
    {
        int result = Syscall.poll(_pollFds, timeout);

        if (result > 0)
        {
            
        }

        return 0;
    }

    public void Write(byte[] data)
    {
        throw new NotImplementedException();
    }
    
    public int Read(byte[] buffer, int index, int count)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        Syscall.close(_socket);
    }
}
