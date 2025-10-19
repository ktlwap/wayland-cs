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
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetSize(int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetSize);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetAnchorRect(int x, int y, int width, int height)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetAnchorRect);
            writer.Write(x);
            writer.Write(y);
            writer.Write(width);
            writer.Write(height);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetAnchor(uint anchor)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetAnchor);
            writer.Write(anchor);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetGravity(uint gravity)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetGravity);
            writer.Write(gravity);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetConstraintAdjustment(uint constraintAdjustment)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetConstraintAdjustment);
            writer.Write(constraintAdjustment);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetOffset(int x, int y)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetOffset);
            writer.Write(x);
            writer.Write(y);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetReactive()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetReactive);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetParentSize(int parentWidth, int parentHeight)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetParentSize);
            writer.Write(parentWidth);
            writer.Write(parentHeight);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetParentConfigure(uint serial)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetParentConfigure);
            writer.Write(serial);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
