using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Subcompositor : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Subcompositor(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        GetSubsurface = 1,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<NewId, ObjectId, ObjectId>? GetSubsurface { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetSubsurface:
                    HandleGetSubsurfaceEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleGetSubsurfaceEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();
            ObjectId arg2 = reader.ReadObjectId();

            GetSubsurface?.Invoke(arg0, arg1, arg2);
        }
        
    }
}
