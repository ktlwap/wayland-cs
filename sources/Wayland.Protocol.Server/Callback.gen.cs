using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class Callback : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Callback(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
        Done = 0,
    }

    private enum RequestOpCode : ushort
    {
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
        public void Done(SocketConnection socketConnection, uint callbackData)
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) EventOpCode.Done);
            writer.Write(callbackData);

            int length = writer.Available;
            writer.Write((byte)(length >> 8));
            writer.Write((byte)(byte.MaxValue & length));

            writer.Flush();
        }

    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            MessageReader reader = socketConnection.MessageReader;
            ushort length = reader.ReadUShort();
            ushort opCode = reader.ReadUShort();
            
            switch (opCode)
            {
            }
        }
        
    }
}
