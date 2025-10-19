using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgWmBase : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgWmBase(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Ping = 0,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        CreatePositioner = 1,
        GetXdgSurface = 2,
        Pong = 3,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Ping(SocketConnection socketConnection, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Ping);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<NewId>? CreatePositioner { get; set; }
        public Action<NewId, ObjectId>? GetXdgSurface { get; set; }
        public Action<uint>? Pong { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.CreatePositioner:
                    HandleCreatePositionerEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetXdgSurface:
                    HandleGetXdgSurfaceEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Pong:
                    HandlePongEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Destroy?.Invoke();
        }
        
        private void HandleCreatePositionerEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            CreatePositioner?.Invoke(arg0);
        }
        
        private void HandleGetXdgSurfaceEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();

            GetXdgSurface?.Invoke(arg0, arg1);
        }
        
        private void HandlePongEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Pong?.Invoke(arg0);
        }
        
    }
}
