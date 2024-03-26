using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

[StructLayout(LayoutKind.Sequential)]
public struct Fd
{
    public readonly int Value;

    public Fd(int value)
    {
        Value = value;
    }
}
