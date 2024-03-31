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
            throw new IOException("Failed to UNIX socket.");
        
        if (Syscall.connect(_socket, new SockaddrUn(path, false)) != 0)
            throw new IOException("Failed to connect to UNIX socket.");
        
        _pollFds = new Pollfd[]
        {
            new Pollfd
            {
                events = PollEvents.POLLIN,
                fd = _socket
            }
        };
    }

    private void Read(int timeout)
    {
        var pol = Syscall.poll(_pollFds, timeout);
    }

    public void Write(byte[] data)
    {
        //_binaryWriter.Write(data);
    }
    
    public int Read(byte[] buffer, int index, int count)
    {
        //return _binaryReader.Read(buffer, index, count);
    }
    
    public void Dispose()
    {
        Syscall.close(_socket);
    }
}
