using Wayland.Protocol.Common;

namespace Wayland.Protocol.Server;

public sealed class DataDeviceManager : ProtocolObject
{
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDeviceManager(uint id, uint version) : base(id, version)
    {
        Events = new EventsWrapper(this);
        Requests = new RequestsWrapper(this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateDataSource = 0,
        GetDataDevice = 1,
    }

    public class EventsWrapper(ProtocolObject protocolObject)
    {
    }

    public class RequestsWrapper(ProtocolObject protocolObject)
    {
        public Action<NewId>? CreateDataSource { get; set; }
        public Action<NewId, ObjectId>? GetDataDevice { get; set; }
        
        internal void HandleEvent(SocketConnection socketConnection)
        {
            ushort length = socketConnection.ReadUInt16();
            ushort opCode = socketConnection.ReadUInt16();
            
            switch (opCode)
            {
                case (ushort) RequestOpCode.CreateDataSource:
                    HandleCreateDataSourceEvent(socketConnection, length);
                    return;
                case (ushort) RequestOpCode.GetDataDevice:
                    HandleGetDataDeviceEvent(socketConnection, length);
                    return;
            }
        }
        
        private void HandleCreateDataSourceEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();

            CreateDataSource?.Invoke(arg0);
        }
        
        private void HandleGetDataDeviceEvent(SocketConnection socketConnection, ushort length)
        {
            byte[] buffer = new byte[length];
            socketConnection.Read(buffer, 0, buffer.Length);

            MessageReader reader = new MessageReader(buffer);

            NewId arg0 = reader.ReadNewId();
            ObjectId arg1 = reader.ReadObjectId();

            GetDataDevice?.Invoke(arg0, arg1);
        }
        
    }
}
