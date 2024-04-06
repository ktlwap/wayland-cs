using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Region : ProtocolObject
{
    public new const string Name = "wl_region";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Region(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public void Destroy()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void Add(int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Add);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void Subtract(int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Subtract);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
