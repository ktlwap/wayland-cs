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
        MessageReader reader = _socketConnection.MessageReader;
        if (!reader.IsDataAvailable)
            return false;

        Span<byte> header = stackalloc byte[8];
        reader.ReadFixed(header, 0, header.Length);

        uint objectId = 0;
        objectId |= (uint)(header[0] << 0);
        objectId |= (uint)(header[1] << 8);
        objectId |= (uint)(header[2] << 16);
        objectId |= (uint)(header[3] << 24);

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
