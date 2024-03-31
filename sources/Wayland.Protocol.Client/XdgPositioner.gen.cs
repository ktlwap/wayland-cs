using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class XdgPositioner : ProtocolObject
{
    public new const string Name = "xdg_positioner";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public XdgPositioner(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        Destroy = 0,
        SetSize = 1,
        SetAnchorRect = 2,
        SetAnchor = 3,
        SetGravity = 4,
        SetConstraintAdjustment = 5,
        SetOffset = 6,
        SetReactive = 7,
        SetParentSize = 8,
        SetParentConfigure = 9,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Destroy()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetSize(int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetSize);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetAnchorRect(int x, int y, int width, int height)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetAnchorRect);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetAnchor(uint anchor)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetAnchor);
            writer.Write(anchor);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetGravity(uint gravity)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetGravity);
            writer.Write(gravity);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetConstraintAdjustment(uint constraintAdjustment)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetConstraintAdjustment);
            writer.Write(constraintAdjustment);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetOffset(int x, int y)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetOffset);
            writer.Write(x);
            writer.Write(y);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetReactive()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetReactive);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetParentSize(int parentWidth, int parentHeight)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetParentSize);
            writer.Write(parentWidth);
            writer.Write(parentHeight);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

        public void SetParentConfigure(uint serial)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetParentConfigure);
            writer.Write(serial);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

    }
}
