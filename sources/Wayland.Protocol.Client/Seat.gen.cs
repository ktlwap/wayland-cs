using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

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
        public Action<uint>? Capabilities { get; set; }
        public Action<string>? Name { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Capabilities:
                    HandleCapabilitiesEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Name:
                    HandleNameEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleCapabilitiesEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Capabilities?.Invoke(arg0);
        }
        
        private void HandleNameEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Name?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void GetPointer(SocketConnection socketConnection, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetPointer);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void GetKeyboard(SocketConnection socketConnection, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetKeyboard);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void GetTouch(SocketConnection socketConnection, NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetTouch);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

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
