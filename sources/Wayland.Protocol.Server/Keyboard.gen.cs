using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Keyboard : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Keyboard(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Keymap = 0,
        Enter = 1,
        Leave = 2,
        Key = 3,
        Modifiers = 4,
        RepeatInfo = 5,
    }

    private enum RequestOpCode : ushort
    {
        Release = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Keymap(SocketConnection socketConnection, uint format, Fd fd, uint size)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Keymap);
            writer.Write(format);
            writer.Write(fd.Value);
            writer.Write(size);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Enter(SocketConnection socketConnection, uint serial, ObjectId surface, byte[] keys)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(keys);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Leave(SocketConnection socketConnection, uint serial, ObjectId surface)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Leave);
            writer.Write(serial);
            writer.Write(surface.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Key(SocketConnection socketConnection, uint serial, uint time, uint key, uint state)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Key);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(key);
            writer.Write(state);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Modifiers(SocketConnection socketConnection, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Modifiers);
            writer.Write(serial);
            writer.Write(modsDepressed);
            writer.Write(modsLatched);
            writer.Write(modsLocked);
            writer.Write(group);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void RepeatInfo(SocketConnection socketConnection, int rate, int delay)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.RepeatInfo);
            writer.Write(rate);
            writer.Write(delay);

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
