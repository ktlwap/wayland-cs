using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Compositor : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Compositor(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateSurface = 0,
        CreateRegion = 1,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? CreateSurface { get; set; }
        public Action<NewId>? CreateRegion { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreateSurface:
                    HandleCreateSurfaceEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.CreateRegion:
                    HandleCreateRegionEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleCreateSurfaceEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            CreateSurface?.Invoke(arg0);
        }
        
        private void HandleCreateRegionEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            CreateRegion?.Invoke(arg0);
        }
        
    }
}
