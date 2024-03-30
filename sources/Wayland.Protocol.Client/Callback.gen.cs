using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Callback : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Callback(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Done = 0,
    }

    private enum RequestOpCode : ushort
    {
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint>? Done { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Done:
                    HandleDoneEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDoneEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Done?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
    }
}
