using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        public void Enter(SocketConnection socketConnection, ObjectId output)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Enter);
            writer.Write(output.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void Leave(SocketConnection socketConnection, ObjectId output)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Leave);
            writer.Write(output.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void PreferredBufferScale(SocketConnection socketConnection, int factor)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.PreferredBufferScale);
            writer.Write(factor);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void PreferredBufferTransform(SocketConnection socketConnection, uint transform)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.PreferredBufferTransform);
            writer.Write(transform);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<ObjectId, int, int>? Attach { get; set; }
        public Action<int, int, int, int>? Damage { get; set; }
        public Action<NewId>? Frame { get; set; }
        public Action<ObjectId>? SetOpaqueRegion { get; set; }
        public Action<ObjectId>? SetInputRegion { get; set; }
        public Action? Commit { get; set; }
        public Action<int>? SetBufferTransform { get; set; }
        public Action<int>? SetBufferScale { get; set; }
        public Action<int, int, int, int>? DamageBuffer { get; set; }
        public Action<int, int>? Offset { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Attach:
                    HandleAttachEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Damage:
                    HandleDamageEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Frame:
                    HandleFrameEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetOpaqueRegion:
                    HandleSetOpaqueRegionEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetInputRegion:
                    HandleSetInputRegionEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Commit:
                    HandleCommitEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetBufferTransform:
                    HandleSetBufferTransformEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetBufferScale:
                    HandleSetBufferScaleEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.DamageBuffer:
                    HandleDamageBufferEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Offset:
                    HandleOffsetEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleAttachEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();

            Attach?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleDamageEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Damage?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleFrameEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            Frame?.Invoke(arg0);
        }
        
        private void HandleSetOpaqueRegionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetOpaqueRegion?.Invoke(arg0);
        }
        
        private void HandleSetInputRegionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetInputRegion?.Invoke(arg0);
        }
        
        private void HandleCommitEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Commit?.Invoke();
        }
        
        private void HandleSetBufferTransformEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            SetBufferTransform?.Invoke(arg0);
        }
        
        private void HandleSetBufferScaleEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            SetBufferScale?.Invoke(arg0);
        }
        
        private void HandleDamageBufferEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            DamageBuffer?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleOffsetEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            Offset?.Invoke(arg0, arg1);
        }
        
    }
}
