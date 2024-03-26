namespace Wayland.Protocol.Common;

public class MessageWriter
{
    private readonly List<byte> _data = new List<byte>(512);

    private void Write(byte value)
    {
        _data.Add(value);
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
        byte[] data = StringConverter.Convert(value);
        for (int i = 0; i < data.Length; ++i)
        {
            Write(data[i]);
        }
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

    public byte[] ToArray()
    {
        return _data.ToArray();
    }
}