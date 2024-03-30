using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Registry : ProtocolObject
{
    public new const string Name = "wl_registry";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Registry(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        _socketConnection = socketConnection;
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Global = 0,
        GlobalRemove = 1,
    }

    private enum RequestOpCode : ushort
    {
        Bind = 0,
    }

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
            case (ushort) EventOpCode.Global:
                Events.HandleGlobalEvent(length);
                return;
            case (ushort) EventOpCode.GlobalRemove:
                Events.HandleGlobalRemoveEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint, string, uint>? Global { get; set; }
        public Action<uint>? GlobalRemove { get; set; }
        
        internal void HandleGlobalEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            string arg1 = reader.ReadString();
            uint arg2 = reader.ReadUInt();

            Global?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleGlobalRemoveEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            GlobalRemove?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Bind(uint name, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Bind);
            writer.Write(name);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
