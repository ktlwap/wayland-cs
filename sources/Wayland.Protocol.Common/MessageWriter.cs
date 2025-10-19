using System.Net.Sockets;
using System.Text;

namespace Wayland.Protocol.Common;

public class MessageWriter
{
    private readonly SocketConnection _socketConnection;
    private readonly BinaryWriter _binaryWriter;
    
    public int Available { get; private set; }
    
    internal MessageWriter(SocketConnection socketConnection, NetworkStream networkStream)
    {
        _socketConnection = socketConnection;
        _binaryWriter =  new BinaryWriter(networkStream);
    }

    private void Write(byte value)
    {
        _binaryWriter.Write(value);
        ++Available;
    }
    
    public void Write(byte[] data)
    {
        Write(data.Length);
        
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
        
        for (int i = 0; i < 4 - data.Length % 4; ++i)
        {
            Write((byte) 0);
        }
    }

    
    public void Write(short value)
    {
        byte[] data = BitConverter.GetBytes(value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }
    
    public void Write(ushort value)
    {
        byte[] data = BitConverter.GetBytes(value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }
    
    public void Write(int value)
    {
        byte[] data = BitConverter.GetBytes(value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }
    
    public void Write(uint value)
    {
        byte[] data = BitConverter.GetBytes(value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }
    
    public void Write(string value)
    {
        if (!BitConverter.IsLittleEndian)
            throw new Exception("Big Endian systems are not supported yet.");
        
        byte[] data = new byte[sizeof(int) + value.Length + 1];
        byte[] utf8String = Encoding.Default.GetBytes(value);
        int length = utf8String.Length + 1;

        data[0] = (byte)(length << 24);
        data[1] = (byte)(length << 16);
        data[2] = (byte)(length << 8);
        data[3] = (byte)(length << 0);
        Buffer.BlockCopy(utf8String, 0, data, 4, utf8String.Length);
        data[^1] = 0;
        
        Write(data);
    }
    
    public void Write(Fd id)
    {
        Write(id.Value);
    }
    
    public void Write(Fixed id)
    {
        Write(id.Value);
    }
    
    public void Write(NewId id)
    {
        byte[] data = BitConverter.GetBytes(id.Value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }
    
    public void Write(ObjectId id)
    {
        byte[] data = BitConverter.GetBytes(id.Value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
    }

    public void Flush()
    {
        _binaryWriter.Flush();
        Available = 0;
    }
}
