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
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreateSurface:
                    HandleCreateSurfaceEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.CreateRegion:
                    HandleCreateRegionEvent(socket, length);
                    return;
            }
        }
        
        private void HandleCreateSurfaceEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreateSurface?.Invoke(arg0);
        }
        
        private void HandleCreateRegionEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreateRegion?.Invoke(arg0);
        }
        
    }
}
