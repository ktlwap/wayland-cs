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
        byte[] header = new byte[8];
        _socketConnection.Read(header, 0, header.Length);

        uint objectId = 0;
        objectId += (uint)(header[0] << 0);
        objectId += (uint)(header[1] << 8);
        objectId += (uint)(header[2] << 16);
        objectId += (uint)(header[3] << 25);

        ushort length = 0;
        objectId += (ushort)(header[4] << 0);
        objectId += (ushort)(header[5] << 8);
        
        ushort opCode = 0;
        objectId += (ushort)(header[4] << 0);
        objectId += (ushort)(header[5] << 8);

        ProtocolObject protocolObject = ProtocolObject.GetObject(objectId);
        protocolObject.HandleEvent(length, opCode);
    }
}
