namespace Wayland.Protocol.Common;

public abstract class ProtocolObject
{
    public readonly uint Id;
    public readonly uint Version;
    
    public ProtocolObject(uint id, uint version)
    {
        Id = id;
        Version = version;
    }
}
