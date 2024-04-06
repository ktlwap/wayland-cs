using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

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

        public void Configure(Socket socket, uint edges, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
            writer.Write(edges);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void PopupDone(Socket socket)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.PopupDone);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint>? Pong { get; set; }
        public Action<ObjectId, uint>? Move { get; set; }
        public Action<ObjectId, uint, uint>? Resize { get; set; }
        public Action? SetToplevel { get; set; }
        public Action<ObjectId, int, int, uint>? SetTransient { get; set; }
        public Action<uint, uint, ObjectId>? SetFullscreen { get; set; }
        public Action<ObjectId, uint, ObjectId, int, int, uint>? SetPopup { get; set; }
        public Action<ObjectId>? SetMaximized { get; set; }
        public Action<string>? SetTitle { get; set; }
        public Action<string>? SetClass { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Pong:
                    HandlePongEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Move:
                    HandleMoveEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetToplevel:
                    HandleSetToplevelEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetTransient:
                    HandleSetTransientEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetFullscreen:
                    HandleSetFullscreenEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetPopup:
                    HandleSetPopupEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetMaximized:
                    HandleSetMaximizedEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetTitle:
                    HandleSetTitleEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetClass:
                    HandleSetClassEvent(socket, length);
                    return;
            }
        }
        
        private void HandlePongEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Pong?.Invoke(arg0);
        }
        
        private void HandleMoveEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            Move?.Invoke(arg0, arg1);
        }
        
        private void HandleResizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();

            Resize?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetToplevelEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetToplevel?.Invoke();
        }
        
        private void HandleSetTransientEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            uint arg3 = reader.ReadUInt();

            SetTransient?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSetFullscreenEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            ObjectId arg2 = reader.ReadObjectId();

            SetFullscreen?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetPopupEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            ObjectId arg2 = reader.ReadObjectId();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            uint arg5 = reader.ReadUInt();

            SetPopup?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        
        private void HandleSetMaximizedEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetMaximized?.Invoke(arg0);
        }
        
        private void HandleSetTitleEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetTitle?.Invoke(arg0);
        }
        
        private void HandleSetClassEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetClass?.Invoke(arg0);
        }
        
    }
}
