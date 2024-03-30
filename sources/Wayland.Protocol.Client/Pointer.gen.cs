using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Pointer : ProtocolObject
{
    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Pointer(SocketConnection socketConnection, uint id, uint version) : base(id, version, "wl_pointer")
    {
        _socketConnection = socketConnection;
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Enter = 0,
        Leave = 1,
        Motion = 2,
        Button = 3,
        Axis = 4,
        Frame = 5,
        AxisSource = 6,
        AxisStop = 7,
        AxisDiscrete = 8,
        AxisValue120 = 9,
        AxisRelativeDirection = 10,
    }

    private enum RequestOpCode : ushort
    {
        SetCursor = 0,
        Release = 1,
    }

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
            case (ushort) EventOpCode.Enter:
                Events.HandleEnterEvent(length);
                return;
            case (ushort) EventOpCode.Leave:
                Events.HandleLeaveEvent(length);
                return;
            case (ushort) EventOpCode.Motion:
                Events.HandleMotionEvent(length);
                return;
            case (ushort) EventOpCode.Button:
                Events.HandleButtonEvent(length);
                return;
            case (ushort) EventOpCode.Axis:
                Events.HandleAxisEvent(length);
                return;
            case (ushort) EventOpCode.Frame:
                Events.HandleFrameEvent(length);
                return;
            case (ushort) EventOpCode.AxisSource:
                Events.HandleAxisSourceEvent(length);
                return;
            case (ushort) EventOpCode.AxisStop:
                Events.HandleAxisStopEvent(length);
                return;
            case (ushort) EventOpCode.AxisDiscrete:
                Events.HandleAxisDiscreteEvent(length);
                return;
            case (ushort) EventOpCode.AxisValue120:
                Events.HandleAxisValue120Event(length);
                return;
            case (ushort) EventOpCode.AxisRelativeDirection:
                Events.HandleAxisRelativeDirectionEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint, ObjectId, Fixed, Fixed>? Enter { get; set; }
        public Action<uint, ObjectId>? Leave { get; set; }
        public Action<uint, Fixed, Fixed>? Motion { get; set; }
        public Action<uint, uint, uint, uint>? Button { get; set; }
        public Action<uint, uint, Fixed>? Axis { get; set; }
        public Action? Frame { get; set; }
        public Action<uint>? AxisSource { get; set; }
        public Action<uint, uint>? AxisStop { get; set; }
        public Action<uint, int>? AxisDiscrete { get; set; }
        public Action<uint, int>? AxisValue120 { get; set; }
        public Action<uint, uint>? AxisRelativeDirection { get; set; }
        
        internal void HandleEnterEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            Fixed arg2 = reader.ReadFixed();
            Fixed arg3 = reader.ReadFixed();

            Enter?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandleLeaveEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();

            Leave?.Invoke(arg0, arg1);
        }
        
        internal void HandleMotionEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleButtonEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();
            uint arg3 = reader.ReadUInt();

            Button?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandleAxisEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            Fixed arg2 = reader.ReadFixed();

            Axis?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleFrameEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Frame?.Invoke();
        }
        
        internal void HandleAxisSourceEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            AxisSource?.Invoke(arg0);
        }
        
        internal void HandleAxisStopEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();

            AxisStop?.Invoke(arg0, arg1);
        }
        
        internal void HandleAxisDiscreteEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();

            AxisDiscrete?.Invoke(arg0, arg1);
        }
        
        internal void HandleAxisValue120Event(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();

            AxisValue120?.Invoke(arg0, arg1);
        }
        
        internal void HandleAxisRelativeDirectionEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();

            AxisRelativeDirection?.Invoke(arg0, arg1);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void SetCursor(uint serial, ObjectId surface, int hotspotX, int hotspotY)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetCursor);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(hotspotX);
            writer.Write(hotspotY);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Release()
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
