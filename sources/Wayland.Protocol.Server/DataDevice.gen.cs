using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        public void DataOffer(Socket socket, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.DataOffer);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Enter(Socket socket, uint serial, ObjectId surface, Fixed x, Fixed y, ObjectId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(serial);
            writer.Write(surface.Value);
            writer.Write(x.Value);
            writer.Write(y.Value);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Leave(Socket socket)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Leave);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Motion(Socket socket, uint time, Fixed x, Fixed y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Motion);
            writer.Write(time);
            writer.Write(x.Value);
            writer.Write(y.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Drop(Socket socket)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Drop);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Selection(Socket socket, ObjectId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Selection);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<ObjectId, ObjectId, ObjectId, uint>? StartDrag { get; set; }
        public Action<ObjectId, uint>? SetSelection { get; set; }
        public Action? Release { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.StartDrag:
                    HandleStartDragEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetSelection:
                    HandleSetSelectionEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socket, length);
                    return;
            }
        }
        
        private void HandleStartDragEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            ObjectId arg1 = reader.ReadObjectId();
            ObjectId arg2 = reader.ReadObjectId();
            uint arg3 = reader.ReadUInt();

            StartDrag?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSetSelectionEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            SetSelection?.Invoke(arg0, arg1);
        }
        
        private void HandleReleaseEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Release?.Invoke();
        }
        
    }
}
