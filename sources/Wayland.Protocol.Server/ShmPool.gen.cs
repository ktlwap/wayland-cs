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
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreateBuffer:
                    HandleCreateBufferEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socket, length);
                    return;
            }
        }
        
        private void HandleCreateBufferEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            uint arg5 = reader.ReadUInt();

            CreateBuffer?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        
        private void HandleDestroyEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleResizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            Resize?.Invoke(arg0);
        }
        
    }
}
