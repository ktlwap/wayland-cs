using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Seat : ProtocolObject
{
    public const string Name = "wl_seat";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Seat(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        _socketConnection = socketConnection;
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Capabilities = 0,
        Name = 1,
    }

    private enum RequestOpCode : ushort
    {
        GetPointer = 0,
        GetKeyboard = 1,
        GetTouch = 2,
        Release = 3,
    }

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
            case (ushort) EventOpCode.Capabilities:
                Events.HandleCapabilitiesEvent(length);
                return;
            case (ushort) EventOpCode.Name:
                Events.HandleNameEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint>? Capabilities { get; set; }
        public Action<string>? Name { get; set; }
        
        internal void HandleCapabilitiesEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Capabilities?.Invoke(arg0);
        }
        
        internal void HandleNameEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Name?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void GetPointer(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetPointer);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void GetKeyboard(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetKeyboard);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void GetTouch(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetTouch);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Release()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
