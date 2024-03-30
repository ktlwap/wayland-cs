using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class ShellSurface : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public ShellSurface(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Ping = 0,
        Configure = 1,
        PopupDone = 2,
    }

    private enum RequestOpCode : ushort
    {
        Pong = 0,
        Move = 1,
        Resize = 2,
        SetToplevel = 3,
        SetTransient = 4,
        SetFullscreen = 5,
        SetPopup = 6,
        SetMaximized = 7,
        SetTitle = 8,
        SetClass = 9,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint>? Ping { get; set; }
        public Action<uint, int, int>? Configure { get; set; }
        public Action? PopupDone { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Ping:
                    HandlePingEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Configure:
                    HandleConfigureEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.PopupDone:
                    HandlePopupDoneEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandlePingEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Ping?.Invoke(arg0);
        }
        
        private void HandleConfigureEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();

            Configure?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandlePopupDoneEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            PopupDone?.Invoke();
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Pong(SocketConnection socketConnection, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Pong);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Move(SocketConnection socketConnection, ObjectId seat, uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Move);
            writer.Write(seat.Value);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Resize(SocketConnection socketConnection, ObjectId seat, uint serial, uint edges)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Resize);
            writer.Write(seat.Value);
            writer.Write(serial);
            writer.Write(edges);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetToplevel(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetToplevel);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetTransient(SocketConnection socketConnection, ObjectId parent, int x, int y, uint flags)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetTransient);
            writer.Write(parent.Value);
            writer.Write(x);
            writer.Write(y);
            writer.Write(flags);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetFullscreen(SocketConnection socketConnection, uint method, uint framerate, ObjectId output)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetFullscreen);
            writer.Write(method);
            writer.Write(framerate);
            writer.Write(output.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetPopup(SocketConnection socketConnection, ObjectId seat, uint serial, ObjectId parent, int x, int y, uint flags)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetPopup);
            writer.Write(seat.Value);
            writer.Write(serial);
            writer.Write(parent.Value);
            writer.Write(x);
            writer.Write(y);
            writer.Write(flags);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetMaximized(SocketConnection socketConnection, ObjectId output)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMaximized);
            writer.Write(output.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetTitle(SocketConnection socketConnection, string title)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetTitle);
            writer.Write(title);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetClass(SocketConnection socketConnection, string @class)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetClass);
            writer.Write(@class);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
