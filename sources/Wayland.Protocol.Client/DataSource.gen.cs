using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataSource : ProtocolObject
{
    public new const string Name = "wl_data_source";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataSource(Socket socket, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socket, this);
        Requests = new RequestsWrapper(socket, this);
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

    public class EventsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public Action<string>? Target { get; set; }
        public Action<string, Fd>? Send { get; set; }
        public Action? Cancelled { get; set; }
        public Action? DndDropPerformed { get; set; }
        public Action? DndFinished { get; set; }
        public Action<uint>? Action { get; set; }
        
        internal void HandleTargetEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Target?.Invoke(arg0);
        }
        
        internal void HandleSendEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();
            Fd arg1 = reader.ReadFd();

            Send?.Invoke(arg0, arg1);
        }
        
        internal void HandleCancelledEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Cancelled?.Invoke();
        }
        
        internal void HandleDndDropPerformedEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            DndDropPerformed?.Invoke();
        }
        
        internal void HandleDndFinishedEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            DndFinished?.Invoke();
        }
        
        internal void HandleActionEvent(ushort length)
        {
            byte[] buffer = new byte[length];
            socket.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Action?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(Socket socket, ProtocolObject protocolObject)
    {
        public void Offer(string mimeType)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Offer);
            writer.Write(mimeType);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void Destroy()
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

        public void SetActions(uint dndActions)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetActions);
            writer.Write(dndActions);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socket.Write(data);
        }

    }
}
