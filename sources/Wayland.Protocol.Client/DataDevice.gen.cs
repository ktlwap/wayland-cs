using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataDevice : ProtocolObject
{
    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDevice(SocketConnection socketConnection, uint id, uint version) : base(id, version, "wl_data_device")
    {
        _socketConnection = socketConnection;
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

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
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
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            DataOffer?.Invoke(arg0);
        }
        
        internal void HandleEnterEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            ObjectId arg1 = reader.ReadObjectId();
            Fixed arg2 = reader.ReadFixed();
            Fixed arg3 = reader.ReadFixed();
            ObjectId arg4 = reader.ReadObjectId();

            Enter?.Invoke(arg0, arg1, arg2, arg3, arg4);
        }
        
        internal void HandleLeaveEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Leave?.Invoke();
        }
        
        internal void HandleMotionEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleDropEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Drop?.Invoke();
        }
        
        internal void HandleSelectionEvent(ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Selection?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void StartDrag(ObjectId source, ObjectId origin, ObjectId icon, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.StartDrag);
            writer.Write(source.Value);
            writer.Write(origin.Value);
            writer.Write(icon.Value);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetSelection(ObjectId source, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetSelection);
            writer.Write(source.Value);
            writer.Write(serial);

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
