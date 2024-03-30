using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Surface : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Surface(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Enter = 0,
        Leave = 1,
        PreferredBufferScale = 2,
        PreferredBufferTransform = 3,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        Attach = 1,
        Damage = 2,
        Frame = 3,
        SetOpaqueRegion = 4,
        SetInputRegion = 5,
        Commit = 6,
        SetBufferTransform = 7,
        SetBufferScale = 8,
        DamageBuffer = 9,
        Offset = 10,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<ObjectId>? Enter { get; set; }
        public Action<ObjectId>? Leave { get; set; }
        public Action<int>? PreferredBufferScale { get; set; }
        public Action<uint>? PreferredBufferTransform { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Enter:
                    HandleEnterEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Leave:
                    HandleLeaveEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.PreferredBufferScale:
                    HandlePreferredBufferScaleEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.PreferredBufferTransform:
                    HandlePreferredBufferTransformEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleEnterEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Enter?.Invoke(arg0);
        }
        
        private void HandleLeaveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Leave?.Invoke(arg0);
        }
        
        private void HandlePreferredBufferScaleEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            PreferredBufferScale?.Invoke(arg0);
        }
        
        private void HandlePreferredBufferTransformEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            PreferredBufferTransform?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Destroy(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Attach(SocketConnection socketConnection, ObjectId buffer, int x, int y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Attach);
            writer.Write(buffer.Value);
            writer.Write(x);
            writer.Write(y);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Damage(SocketConnection socketConnection, int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Damage);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Frame(SocketConnection socketConnection, NewId callback)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Frame);
            writer.Write(callback.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetOpaqueRegion(SocketConnection socketConnection, ObjectId region)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetOpaqueRegion);
            writer.Write(region.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetInputRegion(SocketConnection socketConnection, ObjectId region)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetInputRegion);
            writer.Write(region.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Commit(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Commit);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetBufferTransform(SocketConnection socketConnection, int transform)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetBufferTransform);
            writer.Write(transform);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetBufferScale(SocketConnection socketConnection, int scale)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetBufferScale);
            writer.Write(scale);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void DamageBuffer(SocketConnection socketConnection, int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.DamageBuffer);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Offset(SocketConnection socketConnection, int x, int y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Offset);
            writer.Write(x);
            writer.Write(y);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
