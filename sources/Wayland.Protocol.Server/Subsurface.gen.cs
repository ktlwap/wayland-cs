using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Subsurface : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Subsurface(uint id, uint version) : base(id, version)
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
        SetPosition = 1,
        PlaceAbove = 2,
        PlaceBelow = 3,
        SetSync = 4,
        SetDesync = 5,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<int, int>? SetPosition { get; set; }
        public Action<ObjectId>? PlaceAbove { get; set; }
        public Action<ObjectId>? PlaceBelow { get; set; }
        public Action? SetSync { get; set; }
        public Action? SetDesync { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetPosition:
                    HandleSetPositionEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.PlaceAbove:
                    HandlePlaceAboveEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.PlaceBelow:
                    HandlePlaceBelowEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetSync:
                    HandleSetSyncEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetDesync:
                    HandleSetDesyncEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleSetPositionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetPosition?.Invoke(arg0, arg1);
        }
        
        private void HandlePlaceAboveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            PlaceAbove?.Invoke(arg0);
        }
        
        private void HandlePlaceBelowEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            PlaceBelow?.Invoke(arg0);
        }
        
        private void HandleSetSyncEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetSync?.Invoke();
        }
        
        private void HandleSetDesyncEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetDesync?.Invoke();
        }
        
    }
}
