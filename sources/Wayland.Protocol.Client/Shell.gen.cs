using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Shell : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Shell(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        GetShellSurface = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
            }
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void GetShellSurface(SocketConnection socketConnection, NewId id, ObjectId surface)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetShellSurface);
            writer.Write(id.Value);
            writer.Write(surface.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
