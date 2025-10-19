using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Seat : ProtocolObject
{
    public new const string Name = "wl_seat";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Seat(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
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
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Capabilities?.Invoke(arg0);
        }
        
        internal void HandleNameEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            Name?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void GetPointer(NewId id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetPointer);
            writer.Write(id.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void GetKeyboard(NewId id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetKeyboard);
            writer.Write(id.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void GetTouch(NewId id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetTouch);
            writer.Write(id.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Release()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
