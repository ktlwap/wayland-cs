using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Shm : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Shm(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Format = 0,
    }

    private enum RequestOpCode : ushort
    {
        CreatePool = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Format(SocketConnection socketConnection, uint format)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Format);
            writer.Write(format);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId, Fd, int>? CreatePool { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreatePool:
                    HandleCreatePoolEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleCreatePoolEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();
            Fd arg1 = reader.ReadFd();
            int arg2 = reader.ReadInt();

            CreatePool?.Invoke(arg0, arg1, arg2);
        }
        
    }
}
