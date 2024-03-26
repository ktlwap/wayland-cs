using System.Text;

namespace Wayland.Protocol.Common;

/// <summary>
/// String converter class for Wayland message strings.
/// </summary>
public static class StringConverter
{
    /// <summary>
    /// Converts managed string into UTF-8 encoded buffer with leading message length prefix and trailing null character.
    /// </summary>
    /// <param name="str">managed UTF-16 string</param>
    /// <returns></returns>
    /// <exception cref="Exception">Big Endian systems are not supported yet</exception>
    public static byte[] Convert(string str)
    {
        if (!BitConverter.IsLittleEndian)
            throw new Exception("Big Endian system are not supported yet.");
        
        byte[] data = new byte[sizeof(int) + str.Length + 1];
        byte[] utf8String = Encoding.Default.GetBytes(str);
        int length = utf8String.Length + 1;

        data[0] = (byte)(length << 24);
        data[1] = (byte)(length << 16);
        data[2] = (byte)(length << 8);
        data[3] = (byte)(length << 0);
        Buffer.BlockCopy(utf8String, 0, data, 4, utf8String.Length);
        data[^1] = 0;
        
        return data;
    }
    
    /// <summary>
    /// Transform a UTF-8 encoded buffer (including leading message length prefix and trailing null character) into a string.
    /// </summary>
    /// <param name="data">UTF-8 encoded buffer</param>
    /// <param name="startIndex">index where the string starts (inclusively)</param>
    /// <param name="endIndex">index where the string ends (exclusively)</param>
    /// <returns></returns>
    /// <exception cref="Exception">Big Endian systems are not supported yet</exception>
    public static string Parse(byte[] data, in int startIndex, out int endIndex)
    {
        if (!BitConverter.IsLittleEndian)
            throw new Exception("Big Endian systems are not supported yet.");

        int index = startIndex;
        int length = 0;
        length &= data[index++] << 24;
        length &= data[index++] << 16;
        length &= data[index++] << 8;
        length &= data[index++] << 0;
        
        endIndex = index + length;
#if DEBUG
        if (data.Length < length + 4)
            throw new Exception("Provided buffer is smaller than actually prefixed.");
#endif
        
        return Encoding.UTF8.GetString(data, index, length - 1);
    }
}
