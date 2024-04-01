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
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetSize:
                    HandleSetSizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetAnchorRect:
                    HandleSetAnchorRectEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetAnchor:
                    HandleSetAnchorEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetGravity:
                    HandleSetGravityEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetConstraintAdjustment:
                    HandleSetConstraintAdjustmentEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetOffset:
                    HandleSetOffsetEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetReactive:
                    HandleSetReactiveEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetParentSize:
                    HandleSetParentSizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetParentConfigure:
                    HandleSetParentConfigureEvent(socket, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleSetSizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetAnchorRectEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetAnchorRect?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSetAnchorEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetAnchor?.Invoke(arg0);
        }
        
        private void HandleSetGravityEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetGravity?.Invoke(arg0);
        }
        
        private void HandleSetConstraintAdjustmentEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetConstraintAdjustment?.Invoke(arg0);
        }
        
        private void HandleSetOffsetEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetOffset?.Invoke(arg0, arg1);
        }
        
        private void HandleSetReactiveEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetReactive?.Invoke();
        }
        
        private void HandleSetParentSizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetParentSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetParentConfigureEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            SetParentConfigure?.Invoke(arg0);
        }
        
    }
}
