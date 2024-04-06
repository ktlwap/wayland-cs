using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Registry : ProtocolObject
{
    public new const string Name = "wl_registry";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Registry(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public Action<uint, string, uint>? Global { get; set; }
        public Action<uint>? GlobalRemove { get; set; }
        
        internal void HandleGlobalEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            string arg1 = reader.ReadString();
            uint arg2 = reader.ReadUInt();

            Global?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleGlobalRemoveEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            GlobalRemove?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public void Bind(uint name, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Bind);
            writer.Write(name);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
