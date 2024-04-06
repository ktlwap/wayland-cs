using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class XdgPopup : ProtocolObject
{
    public new const string Name = "xdg_popup";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgPopup(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public Action<int, int, int, int>? Configure { get; set; }
        public Action? PopupDone { get; set; }
        public Action<uint>? Repositioned { get; set; }
        
        internal void HandleConfigureEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Configure?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandlePopupDoneEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            PopupDone?.Invoke();
        }
        
        internal void HandleRepositionedEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Repositioned?.Invoke(arg0);
        }
        
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

        public void Grab(ObjectId seat, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Grab);
            writer.Write(seat.Value);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void Reposition(ObjectId positioner, uint token)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Reposition);
            writer.Write(positioner.Value);
            writer.Write(token);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
