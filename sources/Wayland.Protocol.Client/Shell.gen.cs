using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Shell : ProtocolObject
{
    public new const string Name = "wl_shell";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Shell(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
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
        GetShellSurface = 0,
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
        public void GetShellSurface(NewId id, ObjectId surface)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetShellSurface);
            writer.Write(id.Value);
            writer.Write(surface.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }
}
