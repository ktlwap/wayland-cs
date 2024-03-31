using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Touch : ProtocolObject
{
    public new const string Name = "wl_touch";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Touch(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Down:
                Events.HandleDownEvent(length);
                return;
            case (ushort) EventOpCode.Up:
                Events.HandleUpEvent(length);
                return;
            case (ushort) EventOpCode.Motion:
                Events.HandleMotionEvent(length);
                return;
            case (ushort) EventOpCode.Frame:
                Events.HandleFrameEvent(length);
                return;
            case (ushort) EventOpCode.Cancel:
                Events.HandleCancelEvent(length);
                return;
            case (ushort) EventOpCode.Shape:
                Events.HandleShapeEvent(length);
                return;
            case (ushort) EventOpCode.Orientation:
                Events.HandleOrientationEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint, uint, ObjectId, int, Fixed, Fixed>? Down { get; set; }
        public Action<uint, uint, int>? Up { get; set; }
        public Action<uint, int, Fixed, Fixed>? Motion { get; set; }
        public Action? Frame { get; set; }
        public Action? Cancel { get; set; }
        public Action<int, Fixed, Fixed>? Shape { get; set; }
        public Action<int, Fixed>? Orientation { get; set; }
        
        internal void HandleDownEvent(ushort length)
        {
            byte[] buffer = new byte[length];
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
        
        internal void HandleUpEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            int arg2 = reader.ReadInt();

            Up?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleMotionEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            Fixed arg2 = reader.ReadFixed();
            Fixed arg3 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandleFrameEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Frame?.Invoke();
        }
        
        internal void HandleCancelEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Cancel?.Invoke();
        }
        
        internal void HandleShapeEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Shape?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleOrientationEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            Fixed arg1 = reader.ReadFixed();

            Orientation?.Invoke(arg0, arg1);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Release()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

    }
}
