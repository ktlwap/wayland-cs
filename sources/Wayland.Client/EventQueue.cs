using Wayland.Protocol.Client;
using Wayland.Protocol.Common;

namespace Wayland.Client;

public sealed class EventQueue
{
    private SocketConnection _socketConnection;
    
    internal EventQueue(SocketConnection socketConnection)
    {
        _socketConnection = socketConnection;
    }

    public void Dispatch()
    {
        uint objectId = _socketConnection.ReadUInt32();

        ProtocolObject protocolObject = ProtocolObject.GetObject(objectId);
        protocolObject.HandleEvent();
    }
}