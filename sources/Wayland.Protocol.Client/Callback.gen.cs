using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Callback : ProtocolObject
{
    public new const string Name = "wl_callback";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Callback(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Done = 0,
    }

    private enum RequestOpCode : ushort
    {
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Done:
                Events.HandleDoneEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint>? Done { get; set; }
        
        internal void HandleDoneEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Done?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
    }
}
