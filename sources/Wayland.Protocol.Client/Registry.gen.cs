using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Registry : ProtocolObject
{
    public new const string Name = "wl_registry";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Registry(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
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
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            string arg1 = reader.ReadString();
            uint arg2 = reader.ReadUInt();

            Global?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleGlobalRemoveEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            GlobalRemove?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Bind(uint name, NewId id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Bind);
            writer.Write(name);
            writer.Write(id.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
