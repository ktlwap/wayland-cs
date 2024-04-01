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
        public void Error(Socket socket, ObjectId objectId, uint code, string message)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Error);
            writer.Write(objectId.Value);
            writer.Write(code);
            writer.Write(message);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void DeleteId(Socket socket, uint id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.DeleteId);
            writer.Write(id);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? Sync { get; set; }
        public Action<NewId>? GetRegistry { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Sync:
                    HandleSyncEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.GetRegistry:
                    HandleGetRegistryEvent(socket, length);
                    return;
            }
        }
        
        private void HandleSyncEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            Sync?.Invoke(arg0);
        }
        
        private void HandleGetRegistryEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            GetRegistry?.Invoke(arg0);
        }
        
    }
}
