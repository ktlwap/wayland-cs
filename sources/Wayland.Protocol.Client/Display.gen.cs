using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Display : ProtocolObject
{
    public new const string Name = "wl_display";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Display(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Error:
                Events.HandleErrorEvent(length);
                return;
            case (ushort) EventOpCode.DeleteId:
                Events.HandleDeleteIdEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<ObjectId, uint, string>? Error { get; set; }
        public Action<uint>? DeleteId { get; set; }
        
        internal void HandleErrorEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            string arg2 = reader.ReadString();

            Error?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleDeleteIdEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            DeleteId?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Sync(NewId callback)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Sync);
            writer.Write(callback.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void GetRegistry(NewId registry)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetRegistry);
            writer.Write(registry.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
