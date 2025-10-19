using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class XdgPopup : ProtocolObject
{
    public new const string Name = "xdg_popup";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgPopup(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Configure = 0,
        PopupDone = 1,
        Repositioned = 2,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        Grab = 1,
        Reposition = 2,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Configure:
                Events.HandleConfigureEvent(length);
                return;
            case (ushort) EventOpCode.PopupDone:
                Events.HandlePopupDoneEvent(length);
                return;
            case (ushort) EventOpCode.Repositioned:
                Events.HandleRepositionedEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<int, int, int, int>? Configure { get; set; }
        public Action? PopupDone { get; set; }
        public Action<uint>? Repositioned { get; set; }
        
        internal void HandleConfigureEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Configure?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandlePopupDoneEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            PopupDone?.Invoke();
        }
        
        internal void HandleRepositionedEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Repositioned?.Invoke(arg0);
        }
        
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

        public void Grab(ObjectId seat, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Grab);
            writer.Write(seat);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Reposition(ObjectId positioner, uint token)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Reposition);
            writer.Write(positioner);
            writer.Write(token);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
