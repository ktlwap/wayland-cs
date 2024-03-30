using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Keyboard : ProtocolObject
{
    public new const string Name = "wl_keyboard";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Keyboard(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        _socketConnection = socketConnection;
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

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
            case (ushort) EventOpCode.Keymap:
                Events.HandleKeymapEvent(length);
                return;
            case (ushort) EventOpCode.Enter:
                Events.HandleEnterEvent(length);
                return;
            case (ushort) EventOpCode.Leave:
                Events.HandleLeaveEvent(length);
                return;
            case (ushort) EventOpCode.Key:
                Events.HandleKeyEvent(length);
                return;
            case (ushort) EventOpCode.Modifiers:
                Events.HandleModifiersEvent(length);
                return;
            case (ushort) EventOpCode.RepeatInfo:
                Events.HandleRepeatInfoEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint, Fd, uint>? Keymap { get; set; }
        public Action<uint, ObjectId, byte[]>? Enter { get; set; }
        public Action<uint, ObjectId>? Leave { get; set; }
        public Action<uint, uint, uint, uint>? Key { get; set; }
        public Action<uint, uint, uint, uint, uint>? Modifiers { get; set; }
        public Action<int, int>? RepeatInfo { get; set; }
        
        internal void HandleKeymapEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fd arg1 = reader.ReadFd();
            uint arg2 = reader.ReadUInt();

            Keymap?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleEnterEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            byte[] arg2 = reader.ReadByteArray();

            Enter?.Invoke(arg0, arg1, arg2);
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
        
        internal void HandleKeyEvent(ushort length)
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
        
        internal void HandleModifiersEvent(ushort length)
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
        
        internal void HandleRepeatInfoEvent(ushort length)
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
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }
}
