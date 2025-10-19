using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class ShmPool : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public ShmPool(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateBuffer = 0,
        Destroy = 1,
        Resize = 2,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId, int, int, int, int, uint>? CreateBuffer { get; set; }
        public Action? Destroy { get; set; }
        public Action<int>? Resize { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreateBuffer:
                    HandleCreateBufferEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleCreateBufferEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            uint arg5 = reader.ReadUInt();

            CreateBuffer?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Destroy?.Invoke();
        }
        
        private void HandleResizeEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();

            Resize?.Invoke(arg0);
        }
        
    }
}
