using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Shell : ProtocolObject
{
    public new const string Name = "wl_shell";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Shell(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        GetShellSurface = 0,
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
        public void GetShellSurface(NewId id, ObjectId surface)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetShellSurface);
            writer.Write(id.Value);
            writer.Write(surface.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
