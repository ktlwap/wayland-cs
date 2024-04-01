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
        public void Enter(Socket socket, uint serial, ObjectId surface, Fixed surfaceX, Fixed surfaceY)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(surfaceX.Value);
            writer.Write(surfaceY.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Leave(Socket socket, uint serial, ObjectId surface)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Leave);
            writer.Write(serial);
            writer.Write(surface.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Motion(Socket socket, uint time, Fixed surfaceX, Fixed surfaceY)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Motion);
            writer.Write(time);
            writer.Write(surfaceX.Value);
            writer.Write(surfaceY.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Button(Socket socket, uint serial, uint time, uint button, uint state)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Button);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(button);
            writer.Write(state);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Axis(Socket socket, uint time, uint axis, Fixed value)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Axis);
            writer.Write(time);
            writer.Write(axis);
            writer.Write(value.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Frame(Socket socket)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Frame);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void AxisSource(Socket socket, uint axisSource)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisSource);
            writer.Write(axisSource);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void AxisStop(Socket socket, uint time, uint axis)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisStop);
            writer.Write(time);
            writer.Write(axis);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void AxisDiscrete(Socket socket, uint axis, int discrete)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisDiscrete);
            writer.Write(axis);
            writer.Write(discrete);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void AxisValue120(Socket socket, uint axis, int value120)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisValue120);
            writer.Write(axis);
            writer.Write(value120);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void AxisRelativeDirection(Socket socket, uint axis, uint direction)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.AxisRelativeDirection);
            writer.Write(axis);
            writer.Write(direction);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, ObjectId, int, int>? SetCursor { get; set; }
        public Action? Release { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.SetCursor:
                    HandleSetCursorEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socket, length);
                    return;
            }
        }
        
        private void HandleSetCursorEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            SetCursor?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleReleaseEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Release?.Invoke();
        }
        
    }
}
