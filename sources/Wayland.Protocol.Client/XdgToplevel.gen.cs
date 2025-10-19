using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class XdgToplevel : ProtocolObject
{
    public new const string Name = "xdg_toplevel";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgToplevel(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
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

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Configure:
                Events.HandleConfigureEvent(length);
                return;
            case (ushort) EventOpCode.Close:
                Events.HandleCloseEvent(length);
                return;
            case (ushort) EventOpCode.ConfigureBounds:
                Events.HandleConfigureBoundsEvent(length);
                return;
            case (ushort) EventOpCode.WmCapabilities:
                Events.HandleWmCapabilitiesEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<int, int, byte[]>? Configure { get; set; }
        public Action? Close { get; set; }
        public Action<int, int>? ConfigureBounds { get; set; }
        public Action<byte[]>? WmCapabilities { get; set; }
        
        internal void HandleConfigureEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            byte[] arg2 = reader.ReadByteArray();

            Configure?.Invoke(arg0, arg1, arg2);
        }
        
        internal void HandleCloseEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Close?.Invoke();
        }
        
        internal void HandleConfigureBoundsEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();

            ConfigureBounds?.Invoke(arg0, arg1);
        }
        
        internal void HandleWmCapabilitiesEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            byte[] arg0 = reader.ReadByteArray();

            WmCapabilities?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Destroy()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetParent(ObjectId parent)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetParent);
            writer.Write(parent);

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

        public void SetAppId(string appId)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetAppId);
            writer.Write(appId);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void ShowWindowMenu(ObjectId seat, uint serial, int x, int y)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.ShowWindowMenu);
            writer.Write(seat);
            writer.Write(serial);
            writer.Write(x);
            writer.Write(y);

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
            writer.Write(seat);
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
            writer.Write(seat);
            writer.Write(serial);
            writer.Write(edges);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetMaxSize(int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMaxSize);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetMinSize(int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMinSize);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetMaximized()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMaximized);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void UnsetMaximized()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.UnsetMaximized);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetFullscreen(ObjectId output)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetFullscreen);
            writer.Write(output);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void UnsetFullscreen()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.UnsetFullscreen);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetMinimized()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetMinimized);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
