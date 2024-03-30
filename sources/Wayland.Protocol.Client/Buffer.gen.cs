using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Buffer : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Buffer(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Release = 0,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action? Release { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Release:
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

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Destroy(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
