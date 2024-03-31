using System.Net.Sockets;

namespace Wayland.Protocol.Common;

public sealed class SocketConnection : IDisposable
{
    private readonly Socket _socket;
    private readonly NetworkStream _networkStream;
    private readonly BinaryReader _binaryReader;
    private readonly BinaryWriter _binaryWriter;

    public SocketConnection(string unixSocketPath)
    {
        _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        _socket.Connect(new UnixDomainSocketEndPoint(unixSocketPath));
        _networkStream = new NetworkStream(_socket);
        _binaryReader = new BinaryReader(_networkStream);
        _binaryWriter = new BinaryWriter(_networkStream);
    }
    
    public void Write(byte value)
    {
        _binaryWriter.Write(value);
    }
    
    public void Write(short value)
    {
        _binaryWriter.Write(value);
    }
    
    public void Write(ushort value)
    {
        _binaryWriter.Write(value);
    }
    
    public void Write(int value)
    {
        _binaryWriter.Write(value);
    }
    
    public void Write(uint value)
    {
        _binaryWriter.Write(value);
    }

    public void Write(byte[] data)
    {
        _binaryWriter.Write(data);
    }

    public ushort ReadUInt16()
    {
        return _binaryReader.ReadUInt16();
    }

    public int ReadInt32()
    {
        return _binaryReader.ReadInt32();
    }

    public uint ReadUInt32()
    {
        return _binaryReader.ReadUInt32();
    }
    
    public int Read(byte[] buffer, int index, int count)
    {
        return _binaryReader.Read(buffer, index, count);
    }

    public bool IsDataAvailable()
    {
        return _networkStream.DataAvailable;
    }
    
    public void Dispose()
    {
        _socket.Dispose();
        _networkStream.Dispose();
        _binaryReader.Dispose();
        _binaryWriter.Dispose();
    }
}
