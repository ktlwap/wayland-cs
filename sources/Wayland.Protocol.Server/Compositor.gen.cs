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
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
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
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreateSurface?.Invoke(arg0);
        }
        
        private void HandleCreateRegionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreateRegion?.Invoke(arg0);
        }
        
    }
}
