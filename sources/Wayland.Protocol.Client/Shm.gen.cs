using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Shm : ProtocolObject
{
    public new const string Name = "wl_shm";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Shm(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Format = 0,
    }

    private enum RequestOpCode : ushort
    {
        CreatePool = 0,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Format:
                Events.HandleFormatEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint>? Format { get; set; }
        
        internal void HandleFormatEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Format?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void CreatePool(NewId id, Fd fd, int size)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreatePool);
            writer.Write(id);
            writer.Write(fd);
            writer.Write(size);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
