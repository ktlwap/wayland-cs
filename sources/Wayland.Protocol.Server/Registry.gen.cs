using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Registry : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Registry(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
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

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Global(SocketConnection socketConnection, uint name, string @interface, uint version)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Global);
            writer.Write(name);
            writer.Write(@interface);
            writer.Write(version);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void GlobalRemove(SocketConnection socketConnection, uint name)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.GlobalRemove);
            writer.Write(name);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, NewId>? Bind { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Bind:
                    HandleBindEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleBindEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            NewId arg1 = reader.ReadNewId();

            Bind?.Invoke(arg0, arg1);
        }
        
    }
}
