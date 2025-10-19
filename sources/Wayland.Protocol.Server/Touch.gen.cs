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
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Down);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(surface.Value);
            writer.Write(id);
            writer.Write(x.Value);
            writer.Write(y.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Up(SocketConnection socketConnection, uint serial, uint time, int id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Up);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(id);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Motion(SocketConnection socketConnection, uint time, int id, Fixed x, Fixed y)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Motion);
            writer.Write(time);
            writer.Write(id);
            writer.Write(x.Value);
            writer.Write(y.Value);

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

        public void Cancel(SocketConnection socketConnection)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Cancel);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Shape(SocketConnection socketConnection, int id, Fixed major, Fixed minor)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Shape);
            writer.Write(id);
            writer.Write(major.Value);
            writer.Write(minor.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Orientation(SocketConnection socketConnection, int id, Fixed orientation)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Orientation);
            writer.Write(id);
            writer.Write(orientation.Value);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Release { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleReleaseEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Release?.Invoke();
        }
        
    }
}
