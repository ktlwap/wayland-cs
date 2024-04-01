using Mono.Unix.Native;

namespace Wayland.Protocol.Common;

public sealed class SocketConnection : IDisposable
{
    private readonly int _socket;
    private readonly RingBuffer _readBuffer;
    private readonly RingBuffer _writeBuffer;
    
    public SocketConnection(string path)
    {
        _socket = Syscall.socket(UnixAddressFamily.AF_UNIX, UnixSocketType.SOCK_STREAM, 0);
        if (_socket < 0)
            throw new IOException("Failed to create UNIX socket.");
        
        if (Syscall.connect(_socket, new SockaddrUn(path)) != 0)
            throw new IOException($"Failed to connect to UNIX socket. Reason: {Syscall.GetLastError()}");

        _readBuffer = new RingBuffer(2048 * 8);
        _writeBuffer = new RingBuffer(2048 * 8);
    }

    private int Read(int timeout)
    {
        return 0;
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
