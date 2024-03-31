using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class DataDeviceManager : ProtocolObject
{
    public new const string Name = "wl_data_device_manager";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public DataDeviceManager(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
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
        public void CreateDataSource(NewId id)
        {
            MessageWriter writer = new MessageWriter();
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.CreateDataSource);
            writer.Write(id.Value);

            byte[] data = writer.ToArray();
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

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
            int length = data.Length;
            data[6] = (byte)(byte.MaxValue & length);
            data[7] = (byte)(length >> 8);

            socketConnection.Write(data);
        }

    }
}
