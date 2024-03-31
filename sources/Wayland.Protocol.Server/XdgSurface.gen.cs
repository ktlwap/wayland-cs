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
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
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
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
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
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleGetToplevelEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            GetToplevel?.Invoke(arg0);
        }
        
        private void HandleGetPopupEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();
            ObjectId arg2 = reader.ReadObjectId();

            GetPopup?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetWindowGeometryEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetWindowGeometry?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleAckConfigureEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            AckConfigure?.Invoke(arg0);
        }
        
    }
}
