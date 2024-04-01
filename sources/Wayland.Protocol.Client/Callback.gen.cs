using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Callback : ProtocolObject
{
    public new const string Name = "wl_callback";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Callback(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public Action<uint>? Done { get; set; }
        
        internal void HandleDoneEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Done?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(Socket socket, ProtocolObject protocolObject)
    {
    }
}
