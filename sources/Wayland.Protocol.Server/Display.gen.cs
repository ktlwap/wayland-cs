using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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
        public void Error(SocketConnection socketConnection, ObjectId objectId, uint code, string message)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Error);
            writer.Write(objectId.Value);
            writer.Write(code);
            writer.Write(message);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void DeleteId(SocketConnection socketConnection, uint id)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.DeleteId);
            writer.Write(id);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? Sync { get; set; }
        public Action<NewId>? GetRegistry { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Sync:
                    HandleSyncEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetRegistry:
                    HandleGetRegistryEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleSyncEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            Sync?.Invoke(arg0);
        }
        
        private void HandleGetRegistryEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            GetRegistry?.Invoke(arg0);
        }
        
    }
}
