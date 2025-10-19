using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Seat : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Seat(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Capabilities = 0,
        Name = 1,
    }

    private enum RequestOpCode : ushort
    {
        GetPointer = 0,
        GetKeyboard = 1,
        GetTouch = 2,
        Release = 3,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Capabilities(SocketConnection socketConnection, uint capabilities)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Capabilities);
            writer.Write(capabilities);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Name(SocketConnection socketConnection, string name)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Name);
            writer.Write(name);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? GetPointer { get; set; }
        public Action<NewId>? GetKeyboard { get; set; }
        public Action<NewId>? GetTouch { get; set; }
        public Action? Release { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.GetPointer:
                    HandleGetPointerEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetKeyboard:
                    HandleGetKeyboardEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetTouch:
                    HandleGetTouchEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleGetPointerEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            GetPointer?.Invoke(arg0);
        }
        
        private void HandleGetKeyboardEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            GetKeyboard?.Invoke(arg0);
        }
        
        private void HandleGetTouchEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            NewId arg0 = reader.ReadNewId();

            GetTouch?.Invoke(arg0);
        }
        
        private void HandleReleaseEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Release?.Invoke();
        }
        
    }
}
