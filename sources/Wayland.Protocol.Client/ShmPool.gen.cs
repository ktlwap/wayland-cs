using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class ShmPool : ProtocolObject
{
    public const string Name = "wl_shm_pool";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public ShmPool(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        _socketConnection = socketConnection;
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateBuffer = 0,
        Destroy = 1,
        Resize = 2,
    }

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void CreateBuffer(NewId id, int offset, int width, int height, int stride, uint format)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreateBuffer);
            writer.Write(id.Value);
            writer.Write(offset);
            writer.Write(width);
            writer.Write(height);
            writer.Write(stride);
            writer.Write(format);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Destroy()
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

        public void Resize(int size)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Resize);
            writer.Write(size);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
