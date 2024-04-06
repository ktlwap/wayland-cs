using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Compositor : ProtocolObject
{
    public new const string Name = "wl_compositor";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Compositor(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateSurface = 0,
        CreateRegion = 1,
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
        public void CreateSurface(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreateSurface);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void CreateRegion(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreateRegion);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
