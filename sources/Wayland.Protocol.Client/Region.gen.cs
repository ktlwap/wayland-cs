using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Region : ProtocolObject
{
    public new const string Name = "wl_region";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Region(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        Add = 1,
        Subtract = 2,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Destroy()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Add(int x, int y, int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Add);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Subtract(int x, int y, int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Subtract);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
