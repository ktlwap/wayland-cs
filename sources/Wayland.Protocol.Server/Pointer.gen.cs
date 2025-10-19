using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        public void Enter(SocketConnection socketConnection, uint serial, ObjectId surface, Fixed surfaceX, Fixed surfaceY)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(surfaceX.Value);
            writer.Write(surfaceY.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Leave(SocketConnection socketConnection, uint serial, ObjectId surface)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Leave);
            writer.Write(serial);
            writer.Write(surface.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Motion(SocketConnection socketConnection, uint time, Fixed surfaceX, Fixed surfaceY)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Motion);
            writer.Write(time);
            writer.Write(surfaceX.Value);
            writer.Write(surfaceY.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Button(SocketConnection socketConnection, uint serial, uint time, uint button, uint state)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Button);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(button);
            writer.Write(state);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Axis(SocketConnection socketConnection, uint time, uint axis, Fixed value)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Axis);
            writer.Write(time);
            writer.Write(axis);
            writer.Write(value.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Frame(SocketConnection socketConnection)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Frame);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void AxisSource(SocketConnection socketConnection, uint axisSource)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisSource);
            writer.Write(axisSource);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void AxisStop(SocketConnection socketConnection, uint time, uint axis)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisStop);
            writer.Write(time);
            writer.Write(axis);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void AxisDiscrete(SocketConnection socketConnection, uint axis, int discrete)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisDiscrete);
            writer.Write(axis);
            writer.Write(discrete);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void AxisValue120(SocketConnection socketConnection, uint axis, int value120)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisValue120);
            writer.Write(axis);
            writer.Write(value120);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void AxisRelativeDirection(SocketConnection socketConnection, uint axis, uint direction)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisRelativeDirection);
            writer.Write(axis);
            writer.Write(direction);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, ObjectId, int, int>? SetCursor { get; set; }
        public Action? Release { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.SetCursor:
                    HandleSetCursorEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleSetCursorEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetCursor?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleReleaseEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Release?.Invoke();
        }
        
    }
}
