using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

[StructLayout(LayoutKind.Sequential)]
public struct ObjectId
{
    public readonly uint Value;

    public ObjectId(uint value)
    {
        Value = value;
    }
}
