using Wayland.Protocol.Common;

namespace Wayland.Protocol.Client;

public sealed class Output : ProtocolObject
{
    public new const string Name = "wl_output";

    public readonly EventsWrapper Events;
    public readonly RequestsWrapper Requests;

    public Output(SocketConnection socketConnection, uint id, uint version) : base(id, version, Name)
    {
        Events = new EventsWrapper(socketConnection, this);
        Requests = new RequestsWrapper(socketConnection, this);
    }

    private enum EventOpCode : ushort
    {
        Geometry = 0,
        Mode = 1,
        Done = 2,
        Scale = 3,
        Name = 4,
        Description = 5,
    }

    private enum RequestOpCode : ushort
    {
        Release = 0,
    }

    internal override void HandleEvent(ushort length, ushort opCode)
    {
        switch (opCode)
        {
            case (ushort) EventOpCode.Geometry:
                Events.HandleGeometryEvent(length);
                return;
            case (ushort) EventOpCode.Mode:
                Events.HandleModeEvent(length);
                return;
            case (ushort) EventOpCode.Done:
                Events.HandleDoneEvent(length);
                return;
            case (ushort) EventOpCode.Scale:
                Events.HandleScaleEvent(length);
                return;
            case (ushort) EventOpCode.Name:
                Events.HandleNameEvent(length);
                return;
            case (ushort) EventOpCode.Description:
                Events.HandleDescriptionEvent(length);
                return;
        }
    }

    public class EventsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public Action<int, int, int, int, int, string, string, int>? Geometry { get; set; }
        public Action<uint, int, int, int>? Mode { get; set; }
        public Action? Done { get; set; }
        public Action<int>? Scale { get; set; }
        public Action<string>? Name { get; set; }
        public Action<string>? Description { get; set; }
        
        internal void HandleGeometryEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();
            int arg4 = reader.ReadInt();
            string arg5 = reader.ReadString();
            string arg6 = reader.ReadString();
            int arg7 = reader.ReadInt();

            Geometry?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        internal void HandleModeEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            uint arg0 = reader.ReadUInt();
            int arg1 = reader.ReadInt();
            int arg2 = reader.ReadInt();
            int arg3 = reader.ReadInt();

            Mode?.Invoke(arg0, arg1, arg2, arg3);
        }
        
        internal void HandleDoneEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;


            Done?.Invoke();
        }
        
        internal void HandleScaleEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            int arg0 = reader.ReadInt();

            Scale?.Invoke(arg0);
        }
        
        internal void HandleNameEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            Name?.Invoke(arg0);
        }
        
        internal void HandleDescriptionEvent(ushort length)
        {
            MessageReader reader = socketConnection.MessageReader;

            string arg0 = reader.ReadString();

            Description?.Invoke(arg0);
        }
        
    }

    public class RequestsWrapper(SocketConnection socketConnection, ProtocolObject protocolObject)
    {
        public void Release()
        {
            MessageWriter writer = socketConnection.MessageWriter;
            writer.Write(protocolObject.Id);
            writer.Write((int) RequestOpCode.Release);

            int length = writer.Available;
            writer.Write((byte)(byte.MaxValue & length));
            writer.Write((byte)(length >> 8));

            writer.Flush();
        }

    }
}
