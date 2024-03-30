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
        objectId |= (uint)(header[0] << 0);
        objectId |= (uint)(header[1] << 8);
        objectId |= (uint)(header[2] << 16);
        objectId |= (uint)(header[3] << 25);

        ushort opCode = 0;
        opCode |= (ushort)(header[4] << 0);
        opCode |= (ushort)(header[5] << 8);
        
        ushort length = 0;
        length |= (ushort)(header[6] << 0);
        length |= (ushort)(header[7] << 8);

        ProtocolObject protocolObject = ProtocolObject.GetObject(objectId);
        protocolObject.HandleEvent((ushort)(length - 8), opCode);
    }
}
