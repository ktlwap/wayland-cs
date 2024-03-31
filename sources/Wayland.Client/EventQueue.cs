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

    public bool Dispatch()
    {
        if (!_socketConnection.IsDataAvailable())
            return false;
        
        byte[] header = new byte[8];
        int number = _socketConnection.Read(header, 0, header.Length);
        if (number < 1)
            return false;

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

        return true;
    }
}
