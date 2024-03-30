using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        
        _protocolObjects.Add(id, this);
    }
    
    public static NewId AllocateId()
    {
        return new NewId((uint) _protocolObjects.Count);
    }
    
    public static ProtocolObject GetObject(uint id)
    {
        return _protocolObjects[id];
    }
}
