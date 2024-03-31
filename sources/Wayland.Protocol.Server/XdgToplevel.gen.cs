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
        public void Configure(SocketConnection socketConnection, int width, int height, byte[] states)
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

            socketConnection.Write(data);
        }

        public void Close(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Close);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void ConfigureBounds(SocketConnection socketConnection, int width, int height)
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

            socketConnection.Write(data);
        }

        public void WmCapabilities(SocketConnection socketConnection, byte[] capabilities)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.WmCapabilities);
            writer.Write(capabilities);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(length >> 8);
            data[7] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
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
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetParent:
                    HandleSetParentEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetTitle:
                    HandleSetTitleEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetAppId:
                    HandleSetAppIdEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.ShowWindowMenu:
                    HandleShowWindowMenuEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Move:
                    HandleMoveEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetMaxSize:
                    HandleSetMaxSizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetMinSize:
                    HandleSetMinSizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetMaximized:
                    HandleSetMaximizedEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.UnsetMaximized:
                    HandleUnsetMaximizedEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetFullscreen:
                    HandleSetFullscreenEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.UnsetFullscreen:
                    HandleUnsetFullscreenEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetMinimized:
                    HandleSetMinimizedEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Destroy?.Invoke();
        }
        
        private void HandleSetParentEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetParent?.Invoke(arg0);
        }
        
        private void HandleSetTitleEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetTitle?.Invoke(arg0);
        }
        
        private void HandleSetAppIdEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            SetAppId?.Invoke(arg0);
        }
        
        private void HandleShowWindowMenuEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            ShowWindowMenu?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleMoveEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            Move?.Invoke(arg0, arg1);
        }
        
        private void HandleResizeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();

            Resize?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetMaxSizeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetMaxSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetMinSizeEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            SetMinSize?.Invoke(arg0, arg1);
        }
        
        private void HandleSetMaximizedEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetMaximized?.Invoke();
        }
        
        private void HandleUnsetMaximizedEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            UnsetMaximized?.Invoke();
        }
        
        private void HandleSetFullscreenEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            ObjectId arg0 = reader.ReadObjectId();

            SetFullscreen?.Invoke(arg0);
        }
        
        private void HandleUnsetFullscreenEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            UnsetFullscreen?.Invoke();
        }
        
        private void HandleSetMinimizedEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            SetMinimized?.Invoke();
        }
        
    }
}
