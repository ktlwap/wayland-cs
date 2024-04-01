using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class XdgToplevel : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgToplevel(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Configure = 0,
        Close = 1,
        ConfigureBounds = 2,
        WmCapabilities = 3,
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        SetParent = 1,
        SetTitle = 2,
        SetAppId = 3,
        ShowWindowMenu = 4,
        Move = 5,
        Resize = 6,
        SetMaxSize = 7,
        SetMinSize = 8,
        SetMaximized = 9,
        UnsetMaximized = 10,
        SetFullscreen = 11,
        UnsetFullscreen = 12,
        SetMinimized = 13,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Configure(Socket socket, int width, int height, byte[] states)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
            writer.Write(width);
            writer.Write(height);
            writer.Write(states);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void Close(Socket socket)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Close);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void ConfigureBounds(Socket socket, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.ConfigureBounds);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socket.Write(data);
        }

        public void WmCapabilities(Socket socket, byte[] capabilities)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.WmCapabilities);
            writer.Write(capabilities);

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
        public Action<ObjectId>? SetParent { get; set; }
        public Action<string>? SetTitle { get; set; }
        public Action<string>? SetAppId { get; set; }
        public Action<ObjectId, uint, int, int>? ShowWindowMenu { get; set; }
        public Action<ObjectId, uint>? Move { get; set; }
        public Action<ObjectId, uint, uint>? Resize { get; set; }
        public Action<int, int>? SetMaxSize { get; set; }
        public Action<int, int>? SetMinSize { get; set; }
        public Action? SetMaximized { get; set; }
        public Action? UnsetMaximized { get; set; }
        public Action<ObjectId>? SetFullscreen { get; set; }
        public Action? UnsetFullscreen { get; set; }
        public Action? SetMinimized { get; set; }
        
        internal void HandleEvent(Socket socket)
        {
            ushort length = socket.ReadUInt16();
            ushort opCode = socket.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetParent:
                    HandleSetParentEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetTitle:
                    HandleSetTitleEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetAppId:
                    HandleSetAppIdEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.ShowWindowMenu:
                    HandleShowWindowMenuEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Move:
                    HandleMoveEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetMaxSize:
                    HandleSetMaxSizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetMinSize:
                    HandleSetMinSizeEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetMaximized:
                    HandleSetMaximizedEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.UnsetMaximized:
                    HandleUnsetMaximizedEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetFullscreen:
                    HandleSetFullscreenEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.UnsetFullscreen:
                    HandleUnsetFullscreenEvent(socket, length);
                    return;
                case (ushort) RequestOpCode.SetMinimized:
                    HandleSetMinimizedEvent(socket, length);
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
        
        private void HandleSetParentEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetParent?.Invoke(arg0);
        }
        
        private void HandleSetTitleEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetTitle?.Invoke(arg0);
        }
        
        private void HandleSetAppIdEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetAppId?.Invoke(arg0);
        }
        
        private void HandleShowWindowMenuEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            ShowWindowMenu?.Invoke(arg0, arg1, arg2, arg3);
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
        
        private void HandleSetMaxSizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetMaxSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetMinSizeEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetMinSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetMaximizedEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetMaximized?.Invoke();
        }
        
        private void HandleUnsetMaximizedEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            UnsetMaximized?.Invoke();
        }
        
        private void HandleSetFullscreenEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetFullscreen?.Invoke(arg0);
        }
        
        private void HandleUnsetFullscreenEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            UnsetFullscreen?.Invoke();
        }
        
        private void HandleSetMinimizedEvent(Socket socket, ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetMinimized?.Invoke();
        }
        
    }
}
