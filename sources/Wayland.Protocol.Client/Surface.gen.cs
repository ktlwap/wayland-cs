using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Surface : ProtocolObject
{
    public new const string Name = "wl_surface";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Surface(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Enter:
                Events.HandleEnterEvent(length);
                return;
            case (ushort) EventOpCode.Leave:
                Events.HandleLeaveEvent(length);
                return;
            case (ushort) EventOpCode.PreferredBufferScale:
                Events.HandlePreferredBufferScaleEvent(length);
                return;
            case (ushort) EventOpCode.PreferredBufferTransform:
                Events.HandlePreferredBufferTransformEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<ObjectId>? Enter { get; set; }
        public Action<ObjectId>? Leave { get; set; }
        public Action<int>? PreferredBufferScale { get; set; }
        public Action<uint>? PreferredBufferTransform { get; set; }
        
        internal void HandleEnterEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Enter?.Invoke(arg0);
        }
        
        internal void HandleLeaveEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            Leave?.Invoke(arg0);
        }
        
        internal void HandlePreferredBufferScaleEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            PreferredBufferScale?.Invoke(arg0);
        }
        
        internal void HandlePreferredBufferTransformEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            PreferredBufferTransform?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
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

            socketConnection.Write(data);
        }

        public void Attach(ObjectId buffer, int x, int y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Attach);
            writer.Write(buffer.Value);
            writer.Write(x);
            writer.Write(y);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void Damage(int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Damage);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void Frame(NewId callback)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Frame);
            writer.Write(callback.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetOpaqueRegion(ObjectId region)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetOpaqueRegion);
            writer.Write(region.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetInputRegion(ObjectId region)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetInputRegion);
            writer.Write(region.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void Commit()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Commit);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetBufferTransform(int transform)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetBufferTransform);
            writer.Write(transform);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetBufferScale(int scale)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetBufferScale);
            writer.Write(scale);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void DamageBuffer(int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.DamageBuffer);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void Offset(int x, int y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Offset);
            writer.Write(x);
            writer.Write(y);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

    }
}
