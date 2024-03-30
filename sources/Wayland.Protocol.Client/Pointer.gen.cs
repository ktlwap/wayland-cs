using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Pointer : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Pointer(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
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

    public class EventsWrapper(ProtocolObject protocolObject)
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
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Enter:
                    HandleEnterEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Leave:
                    HandleLeaveEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Motion:
                    HandleMotionEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Button:
                    HandleButtonEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Axis:
                    HandleAxisEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Frame:
                    HandleFrameEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.AxisSource:
                    HandleAxisSourceEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.AxisStop:
                    HandleAxisStopEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.AxisDiscrete:
                    HandleAxisDiscreteEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.AxisValue120:
                    HandleAxisValue120Event(socketConnection, length);
                    return;
                case (ushort) EventOpCode.AxisRelativeDirection:
                    HandleAxisRelativeDirectionEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleEnterEvent(SocketConnection socketConnection, ushort length)
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
        
        private void HandleLeaveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();

            Leave?.Invoke(arg0, arg1);
        }
        
        private void HandleMotionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleButtonEvent(SocketConnection socketConnection, ushort length)
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
        
        private void HandleAxisEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            Fixed arg2 = reader.ReadFixed();

            Axis?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleFrameEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Frame?.Invoke();
        }
        
        private void HandleAxisSourceEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            AxisSource?.Invoke(arg0);
        }
        
        private void HandleAxisStopEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();

            AxisStop?.Invoke(arg0, arg1);
        }
        
        private void HandleAxisDiscreteEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();

            AxisDiscrete?.Invoke(arg0, arg1);
        }
        
        private void HandleAxisValue120Event(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();

            AxisValue120?.Invoke(arg0, arg1);
        }
        
        private void HandleAxisRelativeDirectionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();

            AxisRelativeDirection?.Invoke(arg0, arg1);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void SetCursor(SocketConnection socketConnection, uint serial, ObjectId surface, int hotspotX, int hotspotY)
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
