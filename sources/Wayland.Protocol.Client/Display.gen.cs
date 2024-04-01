using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Display : ProtocolObject
{
    public new const string Name = "wl_display";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Display(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public Action<ObjectId, uint, string>? Error { get; set; }
        public Action<uint>? DeleteId { get; set; }
        
        internal void HandleErrorEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            string arg2 = reader.ReadString();

            Error?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleDeleteIdEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            DeleteId?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public void Sync(NewId callback)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Sync);
            writer.Write(callback.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void GetRegistry(NewId registry)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetRegistry);
            writer.Write(registry.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
