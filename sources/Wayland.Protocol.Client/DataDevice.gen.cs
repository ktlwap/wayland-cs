using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataDevice : ProtocolObject
{
    public new const string Name = "wl_data_device";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDevice(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        DataOffer = 0,
        Enter = 1,
        Leave = 2,
        Motion = 3,
        Drop = 4,
        Selection = 5,
    }

    private enum RequestOpCode : ushort
    {
        StartDrag = 0,
        SetSelection = 1,
        Release = 2,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.DataOffer:
                Events.HandleDataOfferEvent(length);
                return;
            case (ushort) EventOpCode.Enter:
                Events.HandleEnterEvent(length);
                return;
            case (ushort) EventOpCode.Leave:
                Events.HandleLeaveEvent(length);
                return;
            case (ushort) EventOpCode.Motion:
                Events.HandleMotionEvent(length);
                return;
            case (ushort) EventOpCode.Drop:
                Events.HandleDropEvent(length);
                return;
            case (ushort) EventOpCode.Selection:
                Events.HandleSelectionEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<NewId>? DataOffer { get; set; }
        public Action<uint, ObjectId, Fixed, Fixed, ObjectId>? Enter { get; set; }
        public Action? Leave { get; set; }
        public Action<uint, Fixed, Fixed>? Motion { get; set; }
        public Action? Drop { get; set; }
        public Action<ObjectId>? Selection { get; set; }
        
        internal void HandleDataOfferEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            DataOffer?.Invoke(arg0);
        }
        
        internal void HandleEnterEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            Fixed arg2 = reader.ReadFixed();
            Fixed arg3 = reader.ReadFixed();
            ObjectId arg4 = reader.ReadObjectId();

            Enter?.Invoke(arg0, arg1, arg2, arg3, arg4);
        }
        
        internal void HandleLeaveEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Leave?.Invoke();
        }
        
        internal void HandleMotionEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleDropEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Drop?.Invoke();
        }
        
        internal void HandleSelectionEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();

            Selection?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void StartDrag(ObjectId source, ObjectId origin, ObjectId icon, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.StartDrag);
            writer.Write(source.Value);
            writer.Write(origin.Value);
            writer.Write(icon.Value);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetSelection(ObjectId source, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetSelection);
            writer.Write(source.Value);
            writer.Write(serial);

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
