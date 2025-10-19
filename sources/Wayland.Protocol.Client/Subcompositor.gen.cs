using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Subcompositor : ProtocolObject
{
    public new const string Name = "wl_subcompositor";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Subcompositor(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        GetSubsurface = 1,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Destroy()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void GetSubsurface(NewId id, ObjectId surface, ObjectId parent)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetSubsurface);
            writer.Write(id.Value);
            writer.Write(surface.Value);
            writer.Write(parent.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
