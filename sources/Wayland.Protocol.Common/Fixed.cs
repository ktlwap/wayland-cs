using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

[StructLayout(LayoutKind.Sequential)]
public struct Fixed
{
    public readonly int Value;

    public Fixed(int value)
    {
        Value = value;
    }
}
