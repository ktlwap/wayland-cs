using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Output : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Output(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Geometry = 0,
        Mode = 1,
        Done = 2,
        Scale = 3,
        Name = 4,
        Description = 5,
    }

    private enum RequestOpCode : ushort
    {
        Release = 0,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<int, int, int, int, int, string, string, int>? Geometry { get; set; }
        public Action<uint, int, int, int>? Mode { get; set; }
        public Action? Done { get; set; }
        public Action<int>? Scale { get; set; }
        public Action<string>? Name { get; set; }
        public Action<string>? Description { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Geometry:
                    HandleGeometryEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Mode:
                    HandleModeEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Done:
                    HandleDoneEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Scale:
                    HandleScaleEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Name:
                    HandleNameEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Description:
                    HandleDescriptionEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleGeometryEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            string arg5 = reader.ReadString();
            string arg6 = reader.ReadString();
            int arg7 = reader.ReadInt();

            Geometry?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        private void HandleModeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Mode?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleDoneEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Done?.Invoke();
        }
        
        private void HandleScaleEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();

            Scale?.Invoke(arg0);
        }
        
        private void HandleNameEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Name?.Invoke(arg0);
        }
        
        private void HandleDescriptionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Description?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Release(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
