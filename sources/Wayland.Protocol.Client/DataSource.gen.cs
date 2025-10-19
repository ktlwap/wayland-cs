using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataSource : ProtocolObject
{
    public new const string Name = "wl_data_source";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataSource(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Target = 0,
        Send = 1,
        Cancelled = 2,
        DndDropPerformed = 3,
        DndFinished = 4,
        Action = 5,
    }

    private enum RequestOpCode : ushort
    {
        Offer = 0,
        Destroy = 1,
        SetActions = 2,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Target:
                Events.HandleTargetEvent(length);
                return;
            case (ushort) EventOpCode.Send:
                Events.HandleSendEvent(length);
                return;
            case (ushort) EventOpCode.Cancelled:
                Events.HandleCancelledEvent(length);
                return;
            case (ushort) EventOpCode.DndDropPerformed:
                Events.HandleDndDropPerformedEvent(length);
                return;
            case (ushort) EventOpCode.DndFinished:
                Events.HandleDndFinishedEvent(length);
                return;
            case (ushort) EventOpCode.Action:
                Events.HandleActionEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<string>? Target { get; set; }
        public Action<string, Fd>? Send { get; set; }
        public Action? Cancelled { get; set; }
        public Action? DndDropPerformed { get; set; }
        public Action? DndFinished { get; set; }
        public Action<uint>? Action { get; set; }
        
        internal void HandleTargetEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            Target?.Invoke(arg0);
        }
        
        internal void HandleSendEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();
            Fd arg1 = reader.ReadFd();

            Send?.Invoke(arg0, arg1);
        }
        
        internal void HandleCancelledEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Cancelled?.Invoke();
        }
        
        internal void HandleDndDropPerformedEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            DndDropPerformed?.Invoke();
        }
        
        internal void HandleDndFinishedEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            DndFinished?.Invoke();
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
        public void Offer(string mimeType)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Offer);
            writer.Write(mimeType);

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

        public void SetActions(uint dndActions)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetActions);
            writer.Write(dndActions);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
