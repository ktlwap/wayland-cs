using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        public void Down(SocketConnection socketConnection, uint serial, uint time, ObjectId surface, int id, Fixed x, Fixed y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Down);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(surface.Value);
            writer.Write(id);
            writer.Write(x.Value);
            writer.Write(y.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Up(SocketConnection socketConnection, uint serial, uint time, int id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Up);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(id);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Motion(SocketConnection socketConnection, uint time, int id, Fixed x, Fixed y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Motion);
            writer.Write(time);
            writer.Write(id);
            writer.Write(x.Value);
            writer.Write(y.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Frame(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Frame);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Cancel(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Cancel);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Shape(SocketConnection socketConnection, int id, Fixed major, Fixed minor)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Shape);
            writer.Write(id);
            writer.Write(major.Value);
            writer.Write(minor.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Orientation(SocketConnection socketConnection, int id, Fixed orientation)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Orientation);
            writer.Write(id);
            writer.Write(orientation.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Release { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleReleaseEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Release?.Invoke();
        }
        
    }
}
