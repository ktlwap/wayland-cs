using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Region : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Region(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        Add = 1,
        Subtract = 2,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<int, int, int, int>? Add { get; set; }
        public Action<int, int, int, int>? Subtract { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Add:
                    HandleAddEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Subtract:
                    HandleSubtractEvent(socket, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleAddEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Add?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSubtractEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Subtract?.Invoke(arg0, arg1, arg2, arg3);
        }
        
    }
}
