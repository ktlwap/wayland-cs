using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Keyboard : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Keyboard(SocketConnection socketConnection, uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint, Fd, uint>? Keymap { get; set; }
        public Action<uint, ObjectId, byte[]>? Enter { get; set; }
        public Action<uint, ObjectId>? Leave { get; set; }
        public Action<uint, uint, uint, uint>? Key { get; set; }
        public Action<uint, uint, uint, uint, uint>? Modifiers { get; set; }
        public Action<int, int>? RepeatInfo { get; set; }
        
        internal void HandleEvent()
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Keymap:
                    HandleKeymapEvent(length);
                    return;
                case (ushort) EventOpCode.Enter:
                    HandleEnterEvent(length);
                    return;
                case (ushort) EventOpCode.Leave:
                    HandleLeaveEvent(length);
                    return;
                case (ushort) EventOpCode.Key:
                    HandleKeyEvent(length);
                    return;
                case (ushort) EventOpCode.Modifiers:
                    HandleModifiersEvent(length);
                    return;
                case (ushort) EventOpCode.RepeatInfo:
                    HandleRepeatInfoEvent(length);
                    return;
            }
        }
        
        private void HandleKeymapEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fd arg1 = reader.ReadFd();
            uint arg2 = reader.ReadUInt();

            Keymap?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleEnterEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            byte[] arg2 = reader.ReadByteArray();

            Enter?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleLeaveEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();

            Leave?.Invoke(arg0, arg1);
        }
        
        private void HandleKeyEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();
            uint arg3 = reader.ReadUInt();

            Key?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleModifiersEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();
            uint arg3 = reader.ReadUInt();
            uint arg4 = reader.ReadUInt();

            Modifiers?.Invoke(arg0, arg1, arg2, arg3, arg4);
        }
        
        private void HandleRepeatInfoEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            RepeatInfo?.Invoke(arg0, arg1);
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
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
