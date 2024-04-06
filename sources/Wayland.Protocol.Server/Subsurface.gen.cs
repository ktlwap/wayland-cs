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
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetPosition:
                    HandleSetPositionEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.PlaceAbove:
                    HandlePlaceAboveEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.PlaceBelow:
                    HandlePlaceBelowEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetSync:
                    HandleSetSyncEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetDesync:
                    HandleSetDesyncEvent(socket, length);
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
        
        private void HandleSetPositionEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetPosition?.Invoke(arg0, arg1);
        }
        
        private void HandlePlaceAboveEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            PlaceAbove?.Invoke(arg0);
        }
        
        private void HandlePlaceBelowEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            PlaceBelow?.Invoke(arg0);
        }
        
        private void HandleSetSyncEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetSync?.Invoke();
        }
        
        private void HandleSetDesyncEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetDesync?.Invoke();
        }
        
    }
}
