using System.Text;

namespace Wayland.Protocol.Common;

public class MessageReader
{
    private readonly byte[] _data;
    private int _index;
    
    public MessageReader(byte[] data)
    {
        _data = data;
    }
    
    private byte ReadByte()
    {
        return _data[_index++];
    }
    
    public byte[] ReadByteArray()
    {
        int length = 0;
        length |= ReadByte() << 0;
        length |= ReadByte() << 8;
        length |= ReadByte() << 16;
        length |= ReadByte() << 24;

        byte[] result = new byte[length - 1];
        Buffer.BlockCopy(_data, _index, result, 0, length);
        _index += 4 - _data.Length % 4;

#if DEBUG
        if (_index % 4 != 0)
            throw new Exception("Index is not aligned to 4 bytes.");
#endif
        
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
    
    public string ReadString()
    {
        byte[] data = ReadByteArray();
        return Encoding.UTF8.GetString(data, 0, data.Length - 1);;
    }

    public Fd ReadFd()
    {
        return new Fd(ReadInt());
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
