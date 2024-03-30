using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Display : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Display(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Error = 0,
        DeleteId = 1,
    }

    private enum RequestOpCode : ushort
    {
        Sync = 0,
        GetRegistry = 1,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<ObjectId, uint, string>? Error { get; set; }
        public Action<uint>? DeleteId { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Error:
                    HandleErrorEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.DeleteId:
                    HandleDeleteIdEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleErrorEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            string arg2 = reader.ReadString();

            Error?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleDeleteIdEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            DeleteId?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Sync(SocketConnection socketConnection, NewId callback)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Sync);
            writer.Write(callback.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void GetRegistry(SocketConnection socketConnection, NewId registry)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetRegistry);
            writer.Write(registry.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
