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
        length &= ReadByte() << 24;
        length &= ReadByte() << 16;
        length &= ReadByte() << 8;
        length &= ReadByte() << 0;

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
        return (short)(ReadByte() << 8 | ReadByte());
    }
    
    public ushort ReadUShort()
    {
        return (ushort)(ReadByte() << 8 | ReadByte());
    }
    
    public int ReadInt()
    {
        return ReadByte() << 8 | ReadByte();
    }
    
    public uint ReadUInt()
    {
        return (uint)(ReadByte() << 8 | ReadByte());
    }
    
    public string ReadString()
    {
        int length = 0;
        length &= ReadByte() << 24;
        length &= ReadByte() << 16;
        length &= ReadByte() << 8;
        length &= ReadByte() << 0;
        
        string result = Encoding.UTF8.GetString(_data, _index, length - 1);
        _index += 4 - _data.Length % 4;

#if DEBUG
        if (_index % 4 != 0)
            throw new Exception("Index is not aligned to 4 bytes.");
#endif
        
        return result;
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