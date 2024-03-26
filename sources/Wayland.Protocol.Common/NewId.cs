using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

[StructLayout(LayoutKind.Sequential)]
public struct NewId
{
    public readonly uint Value;

    public NewId(uint value)
    {
        Value = value;
    }
}
