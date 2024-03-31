using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgPopup : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgPopup(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Configure = 0,
        PopupDone = 1,
        Repositioned = 2,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        Grab = 1,
        Reposition = 2,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Configure(SocketConnection socketConnection, int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void PopupDone(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.PopupDone);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Repositioned(SocketConnection socketConnection, uint token)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Repositioned);
            writer.Write(token);

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
        public Action<ObjectId, uint>? Grab { get; set; }
        public Action<ObjectId, uint>? Reposition { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Grab:
                    HandleGrabEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Reposition:
                    HandleRepositionEvent(socketConnection, length);
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
        
        private void HandleGrabEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            Grab?.Invoke(arg0, arg1);
        }
        
        private void HandleRepositionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            Reposition?.Invoke(arg0, arg1);
        }
        
    }
}
