using System.Net.Sockets;
using System.Text;

namespace Wayland.Protocol.Common;

public class MessageReader
{
    private readonly SocketConnection _socketConnection;
    private readonly NetworkStream _networkStream;
    private readonly BinaryReader _binaryReader;
    
    public bool IsDataAvailable => _networkStream.DataAvailable;
    
    internal MessageReader(SocketConnection socketConnection, NetworkStream networkStream)
    {
        _socketConnection = socketConnection;
        _networkStream = networkStream;
        _binaryReader =  new BinaryReader(networkStream);
    }
    
    private byte ReadByte()
    {
        return _binaryReader.ReadByte();
    }

    public void ReadFixed(Span<byte> data, int index, int length, bool skipPadding = true)
    {
        for (int i = index; i < index + length; ++i)
        {
            data[i] = ReadByte();
        }

        if (skipPadding && length % 4 != 0)
        {
            int padding = 4 - length % 4;
            for (int i = 0; i < padding; ++i)
            {
                ReadByte();
            }
        }
    }

    public byte[] ReadByteArray()
    {
        int length = 0;
        length |= ReadByte() << 0;
        length |= ReadByte() << 8;
        length |= ReadByte() << 16;
        length |= ReadByte() << 24;


        byte[] result = new byte[length];
        for (int i = 0; i < length; ++i)
        {
            result[i] = ReadByte();
        }

        if (length % 4 != 0)
        {
            int padding = 4 - length % 4;
            for (int i = 0; i < padding; ++i)
            {
                ReadByte();
            }
        }

        return result;
    }

    public short ReadShort()
    {
        short result = 0;
        result |= (short)(ReadByte() << 0);
        result |= (short)(ReadByte() << 8);
        return result;
    }
    
    public ushort ReadUShort()
    {
        ushort result = 0;
        result |= (ushort)(ReadByte() << 0);
        result |= (ushort)(ReadByte() << 8);
        return result;
    }
    
    public int ReadInt()
    {
        int result = 0;
        result |= ReadByte() << 0;
        result |= ReadByte() << 8;
        result |= ReadByte() << 16;
        result |= ReadByte() << 24;
        return result;
    }
    
    public uint ReadUInt()
    {
        uint result = 0;
        result |= (uint)(ReadByte() << 0);
        result |= (uint)(ReadByte() << 8);
        result |= (uint)(ReadByte() << 16);
        result |= (uint)(ReadByte() << 24);
        return result;
    }
    
    public string? ReadString()
    {
        byte[] data = ReadByteArray();
        if (data.Length > 0)
            return Encoding.UTF8.GetString(data, 0, data.Length);
        else
            return null;
    }

    public Fd ReadFd()
    {
        int fd = _socketConnection.ReadFileDescriptor();
        return new Fd(fd);
    }
    
    public Fixed ReadFixed()
    {
        return new Fixed(ReadInt());
    }
    
    public ObjectId ReadObjectId()
    {
        return new ObjectId(ReadUInt());
    }
    
    public NewId ReadNewId()
    {
        return new NewId(ReadUInt());
    }
}
