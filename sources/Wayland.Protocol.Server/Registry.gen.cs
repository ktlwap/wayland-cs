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
        public void Global(Socket socket, uint name, string @interface, uint version)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Global);
            writer.Write(name);
            writer.Write(@interface);
            writer.Write(version);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void GlobalRemove(Socket socket, uint name)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.GlobalRemove);
            writer.Write(name);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, NewId>? Bind { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Bind:
                    HandleBindEvent(socket, length);
                    return;
            }
        }
        
        private void HandleBindEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            NewId arg1 = reader.ReadNewId();

            Bind?.Invoke(arg0, arg1);
        }
        
    }
}
