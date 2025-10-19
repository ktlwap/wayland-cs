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
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Add:
                    HandleAddEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Subtract:
                    HandleSubtractEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Destroy?.Invoke();
        }
        
        private void HandleAddEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Add?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSubtractEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Subtract?.Invoke(arg0, arg1, arg2, arg3);
        }
        
    }
}
