using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Touch : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Touch(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Down = 0,
        Up = 1,
        Motion = 2,
        Frame = 3,
        Cancel = 4,
        Shape = 5,
        Orientation = 6,
    }

    private enum RequestOpCode : ushort
    {
        Release = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, uint, ObjectId, int, Fixed, Fixed>? Down { get; set; }
        public Action<uint, uint, int>? Up { get; set; }
        public Action<uint, int, Fixed, Fixed>? Motion { get; set; }
        public Action? Frame { get; set; }
        public Action? Cancel { get; set; }
        public Action<int, Fixed, Fixed>? Shape { get; set; }
        public Action<int, Fixed>? Orientation { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Down:
                    HandleDownEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Up:
                    HandleUpEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Motion:
                    HandleMotionEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Frame:
                    HandleFrameEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Cancel:
                    HandleCancelEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Shape:
                    HandleShapeEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Orientation:
                    HandleOrientationEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDownEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            ObjectId arg2 = reader.ReadObjectId();
            int arg3 = reader.ReadInt();
            Fixed arg4 = reader.ReadFixed();
            Fixed arg5 = reader.ReadFixed();

            Down?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        
        private void HandleUpEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            int arg2 = reader.ReadInt();

            Up?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleMotionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            Fixed arg2 = reader.ReadFixed();
            Fixed arg3 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleFrameEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Frame?.Invoke();
        }
        
        private void HandleCancelEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Cancel?.Invoke();
        }
        
        private void HandleShapeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Shape?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleOrientationEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            Fixed arg1 = reader.ReadFixed();

            Orientation?.Invoke(arg0, arg1);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Release(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
