namespace Wayland.Protocol.Common;

/// <summary>
/// Thread safe ring buffer implementation.
/// </summary>
public class RingBuffer
{
    private readonly byte[] _buffer;
    private int _cursor;
    private int _availableBytesCount;
    
    /// <summary>
    /// Returns number of bytes available in the buffer.
    /// </summary>
    public int AvailableBytesCount => _availableBytesCount;
    
    /// <summary>
    /// Returns whether there is any data available in the buffer.
    /// </summary>
    public bool DataAvailable => _availableBytesCount > 0;
    
    /// <summary>
    /// Returns the size of the buffer.
    /// </summary>
    public int Size => _buffer.Length;
    
    /// <summary>
    /// Initializes new RingBuffer with specified size.
    /// </summary>
    /// <param name="size">Size in bytes</param>
    public RingBuffer(int size)
    {
        _buffer = new byte[size];
    }

    private byte ReadByte()
    {
        if (_availableBytesCount < 1)
            throw new Exception("Cannot read more data than available.");
        
        byte value = _buffer[_cursor];
        _buffer[_cursor] = 0;
        
        ++_cursor;
        --_availableBytesCount;
        if (_cursor >= _buffer.Length)
            _cursor = 0;
        
        return value;
    }

    private void WriteByte(byte value)
    {
        if (_buffer[_cursor] != 0)
            throw new Exception("Buffer overflow. Cannot write more data. Try to increase buffer size.");
        
        _buffer[_cursor] = value;
        
        --_cursor;
        ++_availableBytesCount;
        if (_cursor >= _buffer.Length)
            _cursor = 0;
    }
    
    public void Write(byte[] data, int index, int count)
    {
        lock (_buffer)
        {
            for (int i = 0; i < count; ++i)
            {
                WriteByte(data[index + i]);
            }   
        }
    }
    
    public int Read(byte[] data, int index, int count)
    {
        lock (_buffer)
        {
            int numberOfBytesRead = 0;
            for (int i = 0; i < count; ++i)
            {
                if (!DataAvailable)
                    break;
                
                data[index + i] = ReadByte();
                ++numberOfBytesRead;
            }

            return numberOfBytesRead;
        }
    }
}