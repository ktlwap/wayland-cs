using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgPositioner : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgPositioner(uint id, uint version) : base(id, version)
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
        SetSize = 1,
        SetAnchorRect = 2,
        SetAnchor = 3,
        SetGravity = 4,
        SetConstraintAdjustment = 5,
        SetOffset = 6,
        SetReactive = 7,
        SetParentSize = 8,
        SetParentConfigure = 9,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<int, int>? SetSize { get; set; }
        public Action<int, int, int, int>? SetAnchorRect { get; set; }
        public Action<uint>? SetAnchor { get; set; }
        public Action<uint>? SetGravity { get; set; }
        public Action<uint>? SetConstraintAdjustment { get; set; }
        public Action<int, int>? SetOffset { get; set; }
        public Action? SetReactive { get; set; }
        public Action<int, int>? SetParentSize { get; set; }
        public Action<uint>? SetParentConfigure { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetSize:
                    HandleSetSizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetAnchorRect:
                    HandleSetAnchorRectEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetAnchor:
                    HandleSetAnchorEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetGravity:
                    HandleSetGravityEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetConstraintAdjustment:
                    HandleSetConstraintAdjustmentEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetOffset:
                    HandleSetOffsetEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetReactive:
                    HandleSetReactiveEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetParentSize:
                    HandleSetParentSizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetParentConfigure:
                    HandleSetParentConfigureEvent(socketConnection, length);
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
        
        private void HandleSetSizeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetAnchorRectEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetAnchorRect?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSetAnchorEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetAnchor?.Invoke(arg0);
        }
        
        private void HandleSetGravityEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetGravity?.Invoke(arg0);
        }
        
        private void HandleSetConstraintAdjustmentEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetConstraintAdjustment?.Invoke(arg0);
        }
        
        private void HandleSetOffsetEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetOffset?.Invoke(arg0, arg1);
        }
        
        private void HandleSetReactiveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetReactive?.Invoke();
        }
        
        private void HandleSetParentSizeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetParentSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetParentConfigureEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetParentConfigure?.Invoke(arg0);
        }
        
    }
}
