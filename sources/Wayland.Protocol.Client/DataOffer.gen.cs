using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataOffer : ProtocolObject
{
    public new const string Name = "wl_data_offer";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataOffer(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Offer = 0,
        SourceActions = 1,
        Action = 2,
    }

    private enum RequestOpCode : ushort
    {
        Accept = 0,
        Receive = 1,
        Destroy = 2,
        Finish = 3,
        SetActions = 4,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Offer:
                Events.HandleOfferEvent(length);
                return;
            case (ushort) EventOpCode.SourceActions:
                Events.HandleSourceActionsEvent(length);
                return;
            case (ushort) EventOpCode.Action:
                Events.HandleActionEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<string>? Offer { get; set; }
        public Action<uint>? SourceActions { get; set; }
        public Action<uint>? Action { get; set; }
        
        internal void HandleOfferEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            Offer?.Invoke(arg0);
        }
        
        internal void HandleSourceActionsEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            SourceActions?.Invoke(arg0);
        }
        
        internal void HandleActionEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();

            Action?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Accept(uint serial, string mimeType)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Accept);
            writer.Write(serial);
            writer.Write(mimeType);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void Receive(string mimeType, Fd fd)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Receive);
            writer.Write(mimeType);
            writer.Write(fd);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

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

        public void Finish()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Finish);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

        public void SetActions(uint dndActions, uint preferredAction)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetActions);
            writer.Write(dndActions);
            writer.Write(preferredAction);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
