using Wayland.Protocol.Client;
using Wayland.Protocol.Common;

namespace Wayland.Client;

public sealed class EventQueue
{
    private Socket Socket { get; }
    
    internal EventQueue(Socket socket)
    {
        Socket = socket;
    }

    public bool Dispatch()
    {
        byte[] header = new byte[8];
        int number = Socket.Read(header, 0, header.Length);
        if (number < 1)
            return false;

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
