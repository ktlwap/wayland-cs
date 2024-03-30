namespace Wayland.Protocol.Common;

public abstract class ProtocolObject
{
    private static Dictionary<uint, ProtocolObject> _protocolObjects;
    
    public readonly uint Id;
    public readonly uint Version;

    static ProtocolObject()
    {
        _protocolObjects = new Dictionary<uint, ProtocolObject>();
    }

    public ProtocolObject(uint id, uint version)
    {
        Id = id;
        Version = version;
    }

    public static NewId AllocateId()
    {
        return new NewId((uint) _protocolObjects.Count);
    }
}
