using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgWmBase : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgWmBase(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Ping = 0,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        CreatePositioner = 1,
        GetXdgSurface = 2,
        Pong = 3,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Ping(Socket socket, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Ping);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action? Destroy { get; set; }
        public Action<NewId>? CreatePositioner { get; set; }
        public Action<NewId, ObjectId>? GetXdgSurface { get; set; }
        public Action<uint>? Pong { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.CreatePositioner:
                    HandleCreatePositionerEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.GetXdgSurface:
                    HandleGetXdgSurfaceEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Pong:
                    HandlePongEvent(socket, length);
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
        
        private void HandleCreatePositionerEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreatePositioner?.Invoke(arg0);
        }
        
        private void HandleGetXdgSurfaceEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();

            GetXdgSurface?.Invoke(arg0, arg1);
        }
        
        private void HandlePongEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Pong?.Invoke(arg0);
        }
        
    }
}
