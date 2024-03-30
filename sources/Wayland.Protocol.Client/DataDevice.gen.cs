using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataDevice : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDevice(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
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

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? DataOffer { get; set; }
        public Action<uint, ObjectId, Fixed, Fixed, ObjectId>? Enter { get; set; }
        public Action? Leave { get; set; }
        public Action<uint, Fixed, Fixed>? Motion { get; set; }
        public Action? Drop { get; set; }
        public Action<ObjectId>? Selection { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.DataOffer:
                    HandleDataOfferEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Enter:
                    HandleEnterEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Leave:
                    HandleLeaveEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Motion:
                    HandleMotionEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Drop:
                    HandleDropEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Selection:
                    HandleSelectionEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDataOfferEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            DataOffer?.Invoke(arg0);
        }
        
        private void HandleEnterEvent(SocketConnection socketConnection, ushort length)
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
        
        private void HandleLeaveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Leave?.Invoke();
        }
        
        private void HandleMotionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            Fixed arg1 = reader.ReadFixed();
            Fixed arg2 = reader.ReadFixed();

            Motion?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleDropEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Drop?.Invoke();
        }
        
        private void HandleSelectionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Selection?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void StartDrag(SocketConnection socketConnection, ObjectId source, ObjectId origin, ObjectId icon, uint serial)
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

        public void SetSelection(SocketConnection socketConnection, ObjectId source, uint serial)
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

        public void Release(SocketConnection socketConnection)
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
