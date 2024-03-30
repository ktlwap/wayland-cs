using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataSource : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataSource(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
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

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public Action<string>? Target { get; set; }
        public Action<string, Fd>? Send { get; set; }
        public Action? Cancelled { get; set; }
        public Action? DndDropPerformed { get; set; }
        public Action? DndFinished { get; set; }
        public Action<uint>? Action { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) EventOpCode.Target:
                    HandleTargetEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Send:
                    HandleSendEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Cancelled:
                    HandleCancelledEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.DndDropPerformed:
                    HandleDndDropPerformedEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.DndFinished:
                    HandleDndFinishedEvent(socketConnection, length);
                    return;
                case (ushort) EventOpCode.Action:
                    HandleActionEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleTargetEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();

            Target?.Invoke(arg0);
        }
        
        private void HandleSendEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            string arg0 = reader.ReadString();
            Fd arg1 = reader.ReadFd();

            Send?.Invoke(arg0, arg1);
        }
        
        private void HandleCancelledEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            Cancelled?.Invoke();
        }
        
        private void HandleDndDropPerformedEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            DndDropPerformed?.Invoke();
        }
        
        private void HandleDndFinishedEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);


            DndFinished?.Invoke();
        }
        
        private void HandleActionEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length / 8];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            uint arg0 = reader.ReadUInt();

            Action?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public void Offer(SocketConnection socketConnection, string mimeType)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Offer);
            writer.Write(mimeType);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void Destroy(SocketConnection socketConnection)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Destroy);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

        public void SetActions(SocketConnection socketConnection, uint dndActions)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.SetActions);
            writer.Write(dndActions);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue << 8 & length);

            socketConnection.Write(data);
        }

    }
}
