using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataDeviceManager : ProtocolObject
{
    public new const string Name = "wl_data_device_manager";

    private readonly SocketConnection _socketConnection;
    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDeviceManager(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        _socketConnection = socketConnection;
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
    }

    private enum RequestOpCode : ushort
    {
        CreateDataSource = 0,
        GetDataDevice = 1,
    }

    internal override void HandleEvent()
    {
        ushort length = _socketConnection.ReadUInt16();
        ushort opCode = _socketConnection.ReadUInt16();
        
        switch (opCode)
        {
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void CreateDataSource(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreateDataSource);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

        public void GetDataDevice(NewId id, ObjectId seat)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.GetDataDevice);
            writer.Write(id.Value);
            writer.Write(seat.Value);

            byte[] data = writer.ToArray();
            int length = data.Length - 8;
            data[5] = (byte)(length >> 8);
            data[6] = (byte)(byte.MaxValue & length);

            socketConnection.Write(data);
        }

    }
}
