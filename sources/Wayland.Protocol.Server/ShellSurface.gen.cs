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
        public void Ping(SocketConnection socketConnection, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Ping);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Configure(SocketConnection socketConnection, uint edges, int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Configure);
            writer.Write(edges);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void PopupDone(SocketConnection socketConnection)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.PopupDone);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
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
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Pong:
                    HandlePongEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Move:
                    HandleMoveEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Resize:
                    HandleResizeEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetToplevel:
                    HandleSetToplevelEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetTransient:
                    HandleSetTransientEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetFullscreen:
                    HandleSetFullscreenEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetPopup:
                    HandleSetPopupEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetMaximized:
                    HandleSetMaximizedEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetTitle:
                    HandleSetTitleEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetClass:
                    HandleSetClassEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandlePongEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Pong?.Invoke(arg0);
        }
        
        private void HandleMoveEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();

            Move?.Invoke(arg0, arg1);
        }
        
        private void HandleResizeEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            uint arg2 = reader.ReadUInt();

            Resize?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetToplevelEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            SetToplevel?.Invoke();
        }
        
        private void HandleSetTransientEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            uint arg3 = reader.ReadUInt();

            SetTransient?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        private void HandleSetFullscreenEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();
            ObjectId arg2 = reader.ReadObjectId();

            SetFullscreen?.Invoke(arg0, arg1, arg2);
        }
        
        private void HandleSetPopupEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();
            uint arg1 = reader.ReadUInt();
            ObjectId arg2 = reader.ReadObjectId();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            uint arg5 = reader.ReadUInt();

            SetPopup?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        
        private void HandleSetMaximizedEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            ObjectId arg0 = reader.ReadObjectId();

            SetMaximized?.Invoke(arg0);
        }
        
        private void HandleSetTitleEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            SetTitle?.Invoke(arg0);
        }
        
        private void HandleSetClassEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            SetClass?.Invoke(arg0);
        }
        
    }
}
