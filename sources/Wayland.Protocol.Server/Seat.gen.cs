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
        public void Capabilities(Socket socket, uint capabilities)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Capabilities);
            writer.Write(capabilities);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Name(Socket socket, string name)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Name);
            writer.Write(name);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? GetPointer { get; set; }
        public Action<NewId>? GetKeyboard { get; set; }
        public Action<NewId>? GetTouch { get; set; }
        public Action? Release { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.GetPointer:
                    HandleGetPointerEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.GetKeyboard:
                    HandleGetKeyboardEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.GetTouch:
                    HandleGetTouchEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Release:
                    HandleReleaseEvent(socket, length);
                    return;
            }
        }
        
        private void HandleGetPointerEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            GetPointer?.Invoke(arg0);
        }
        
        private void HandleGetKeyboardEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            GetKeyboard?.Invoke(arg0);
        }
        
        private void HandleGetTouchEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            GetTouch?.Invoke(arg0);
        }
        
        private void HandleReleaseEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Release?.Invoke();
        }
        
    }
}
