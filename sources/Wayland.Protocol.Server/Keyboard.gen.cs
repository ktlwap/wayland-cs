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
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Keymap);
            writer.Write(format);
            writer.Write(fd.Value);
            writer.Write(size);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Enter(SocketConnection socketConnection, uint serial, ObjectId surface, byte[] keys)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(keys);

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

        public void Key(SocketConnection socketConnection, uint serial, uint time, uint key, uint state)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Key);
            writer.Write(serial);
            writer.Write(time);
            writer.Write(key);
            writer.Write(state);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Modifiers(SocketConnection socketConnection, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Modifiers);
            writer.Write(serial);
            writer.Write(modsDepressed);
            writer.Write(modsLatched);
            writer.Write(modsLocked);
            writer.Write(group);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void RepeatInfo(SocketConnection socketConnection, int rate, int delay)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.RepeatInfo);
            writer.Write(rate);
            writer.Write(delay);

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
