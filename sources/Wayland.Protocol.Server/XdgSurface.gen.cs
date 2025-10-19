using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgSurface : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgSurface(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Configure = 0,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        GetToplevel = 1,
        GetPopup = 2,
        SetWindowGeometry = 3,
        AckConfigure = 4,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Configure(SocketConnection socketConnection, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
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
        public Action<NewId>? GetToplevel { get; set; }
        public Action<NewId, ObjectId, ObjectId>? GetPopup { get; set; }
        public Action<int, int, int, int>? SetWindowGeometry { get; set; }
        public Action<uint>? AckConfigure { get; set; }
        
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
                case (ushort) RequestOpCode.GetToplevel:
                    HandleGetToplevelEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetPopup:
                    HandleGetPopupEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetWindowGeometry:
                    HandleSetWindowGeometryEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.AckConfigure:
                    HandleAckConfigureEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Destroy?.Invoke();
        }
        
        private void HandleGetToplevelEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            GetToplevel?.Invoke(arg0);
        }
        
        private void HandleGetPopupEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();
            ObjectId arg2 = reader.ReadObjectId();

            GetPopup?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetWindowGeometryEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetWindowGeometry?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleAckConfigureEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            AckConfigure?.Invoke(arg0);
        }
        
    }
}
