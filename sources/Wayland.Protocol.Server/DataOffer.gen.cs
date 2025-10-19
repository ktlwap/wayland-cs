using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class DataOffer : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataOffer(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
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

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Offer(SocketConnection socketConnection, string mimeType)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Offer);
            writer.Write(mimeType);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void SourceActions(SocketConnection socketConnection, uint sourceActions)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.SourceActions);
            writer.Write(sourceActions);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

        public void Action(SocketConnection socketConnection, uint dndAction)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Action);
            writer.Write(dndAction);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<uint, string>? Accept { get; set; }
        public Action<string, Fd>? Receive { get; set; }
        public Action? Destroy { get; set; }
        public Action? Finish { get; set; }
        public Action<uint, uint>? SetActions { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.Accept:
                    HandleAcceptEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Receive:
                    HandleReceiveEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Destroy:
                    HandleDestroyEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.Finish:
                    HandleFinishEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.SetActions:
                    HandleSetActionsEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleAcceptEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            string arg1 = reader.ReadString();

            Accept?.Invoke(arg0, arg1);
        }
        
        private void HandleReceiveEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();
            Fd arg1 = reader.ReadFd();

            Receive?.Invoke(arg0, arg1);
        }
        
        private void HandleDestroyEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Destroy?.Invoke();
        }
        
        private void HandleFinishEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Finish?.Invoke();
        }
        
        private void HandleSetActionsEvent(SocketConnection socketConnection, ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            uint arg1 = reader.ReadUInt();

            SetActions?.Invoke(arg0, arg1);
        }
        
    }
}
