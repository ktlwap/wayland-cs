using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class ShellSurface : ProtocolObject
{
    public new const string Name = "wl_shell_surface";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public ShellSurface(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Ping:
                Events.HandlePingEvent(length);
                return;
            case (ushort) EventOpCode.Configure:
                Events.HandleConfigureEvent(length);
                return;
            case (ushort) EventOpCode.PopupDone:
                Events.HandlePopupDoneEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<uint>? Ping { get; set; }
        public Action<uint, int, int>? Configure { get; set; }
        public Action? PopupDone { get; set; }
        
        internal void HandlePingEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Ping?.Invoke(arg0);
        }
        
        internal void HandleConfigureEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();

            Configure?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandlePopupDoneEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            PopupDone?.Invoke();
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Pong(uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Pong);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Move(ObjectId seat, uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Move);
            writer.Write(seat.Value);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Resize(ObjectId seat, uint serial, uint edges)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Resize);
            writer.Write(seat.Value);
            writer.Write(serial);
            writer.Write(edges);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetToplevel()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetToplevel);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetTransient(ObjectId parent, int x, int y, uint flags)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetTransient);
            writer.Write(parent.Value);
            writer.Write(x);
            writer.Write(y);
            writer.Write(flags);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetFullscreen(uint method, uint framerate, ObjectId output)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetFullscreen);
            writer.Write(method);
            writer.Write(framerate);
            writer.Write(output.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetPopup(ObjectId seat, uint serial, ObjectId parent, int x, int y, uint flags)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetPopup);
            writer.Write(seat.Value);
            writer.Write(serial);
            writer.Write(parent.Value);
            writer.Write(x);
            writer.Write(y);
            writer.Write(flags);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetMaximized(ObjectId output)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMaximized);
            writer.Write(output.Value);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetTitle(string title)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetTitle);
            writer.Write(title);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetClass(string @class)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetClass);
            writer.Write(@class);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
