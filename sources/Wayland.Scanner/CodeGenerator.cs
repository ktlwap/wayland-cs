using System.Text;
using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

public static class CodeGenerator
{
    private const string ClientFilePath = "../../../../Wayland.Protocol.Client/";
    private const string ServerFilePath = "../../../../Wayland.Protocol.Server/";

    public static void Generate(Protocol protocol)
    {
        var filenames = Directory.GetFiles(ServerFilePath);
        if (!filenames.Any(f =>
                f.EndsWith("Wayland.Protocol.Server.csproj") || f.EndsWith("Wayland.Protocol.Client.csproj")))
            throw new Exception("Output directory is not correct. Please set it to the correct path.");


        foreach (var filename in filenames)
            if (filename.EndsWith(".gen.cs"))
                File.Delete(filename);

        foreach (var @interface in protocol.Interfaces)
        {
            if (@interface.Name is "Shm" or "DataOffer")
                // These are not supported yet due to fd as argument.
                continue;

            using var fileStream = File.Create(Path.Combine(ServerFilePath, @interface.Name + ".gen.cs"));

            var source = GenerateSource(@interface);
            fileStream.Write(source, 0, source.Length);
        }
    }

    private static byte[] GenerateSource(Interface @interface)
    {
        var sb = new StringBuilder();
        sb.Append("namespace Wayland.Protocol.Common;\n");
        sb.Append('\n');
        sb.Append($"public sealed class {@interface.Name} : ProtocolObject\n");
        sb.Append("{\n");
        sb.Append("    public readonly EventsWrapper Events;\n");
        sb.Append("    public readonly RequestsWrapper Requests;\n");
        sb.Append('\n');
        sb.Append($"    public {@interface.Name}(uint id, uint version) : base(id, version)\n");
        sb.Append("    {\n");
        sb.Append("        Events = new EventsWrapper(this);\n");
        sb.Append("        Requests = new RequestsWrapper(this);\n");
        sb.Append("    }\n");
        sb.Append("    \n");
        AddEventOpCodeStub(sb, @interface.Events);
        sb.Append("    \n");
        AddRequestOpCodeStub(sb, @interface.Requests);
        sb.Append("    \n");
        AddEventsWrapperStub(sb, @interface.Events);
        sb.Append("    \n");
        AddRequestsWrapperStub(sb, @interface.Requests);
        sb.Append("}\n");

        return Encoding.Default.GetBytes(sb.ToString());
    }

    private static void AddEventOpCodeStub(StringBuilder sb, List<Event> events)
    {
        sb.Append("    private enum EventOpCode : ushort\n");
        sb.Append("    {\n");
        for (var i = 0; i < events.Count; i++) sb.Append($"        {events[i].Name} = {i},\n");

        sb.Append("    }\n");
    }

    private static void AddRequestOpCodeStub(StringBuilder sb, List<Request> requests)
    {
        sb.Append("    private enum RequestOpCode : ushort\n");
        sb.Append("    {\n");
        for (var i = 0; i < requests.Count; i++) sb.Append($"        {requests[i].Name} = {i},\n");

        sb.Append("    }\n");
    }

    private static void AddEventsWrapperStub(StringBuilder sb, List<Event> events)
    {
        sb.Append("    public class EventsWrapper(ProtocolObject protocolObject)\n");
        sb.Append("    {\n");
        foreach (var @event in events)
            if (@event.Arguments.Count > 0)
                sb.Append(
                    $"        public Action<{string.Join(", ", @event.Arguments.Select(arg => arg.Type))}>? {@event.Name} {{ get; set; }}\n");
            else
                sb.Append(
                    $"        public Action? {@event.Name} {{ get; set; }}\n");

        sb.Append("        \n");
        sb.Append("        internal void HandleEvent(SocketConnection socketConnection)\n");
        sb.Append("        {\n");
        sb.Append("            ushort length = socketConnection.ReadUInt16();\n");
        sb.Append("            ushort opCode = socketConnection.ReadUInt16();\n");
        sb.Append("            \n");
        sb.Append("            switch (opCode)\n");
        sb.Append("            {\n");

        foreach (var @event in events)
        {
            sb.Append($"                case (ushort) EventOpCode.{@event.Name}:\n");
            sb.Append($"                    Handle{@event.Name}Event(socketConnection, length);\n");
            sb.Append("                    return;\n");
        }

        sb.Append("            }\n");
        sb.Append("        }\n");
        sb.Append("        \n");

        foreach (var @event in events)
        {
            sb.Append(
                $"        private void Handle{@event.Name}Event(SocketConnection socketConnection, ushort length)\n");
            sb.Append("        {\n");
            sb.Append("            byte[] buffer = new byte[length / 8];\n");
            sb.Append("            socketConnection.Read(buffer, 0, buffer.Length);\n");
            sb.Append('\n');
            sb.Append("            MessageReader reader = new MessageReader(buffer);\n");
            sb.Append('\n');

            for (var i = 0; i < @event.Arguments.Count; ++i)
            {
                var argument = @event.Arguments[i];
                switch (argument.Type)
                {
                    case "int":
                        sb.Append($"            int arg{i} = reader.ReadInt();\n");
                        break;
                    case "uint":
                        sb.Append($"            uint arg{i} = reader.ReadUInt();\n");
                        break;
                    case "Fixed":
                        sb.Append($"            Fixed arg{i} = reader.ReadFixed();\n");
                        break;
                    case "NewId":
                        sb.Append($"            NewId arg{i} = reader.ReadNewId();\n");
                        break;
                    case "ObjectId":
                        sb.Append($"            ObjectId arg{i} = reader.ReadObjectId();\n");
                        break;
                    case "string":
                        sb.Append($"            string arg{i} = reader.ReadString();\n");
                        break;
                    case "Fd":
                        sb.Append($"            Fd arg{i} = reader.ReadFd();\n");
                        break;
                    case "byte[]":
                        sb.Append($"            byte[] arg{i} = reader.ReadByteArray();\n");
                        break;
                    default:
                        throw new Exception("Unknown type: " + argument.Type);
                }
            }

            sb.Append('\n');
            sb.Append(
                $"            {@event.Name}?.Invoke({string.Join(", ", @event.Arguments.Select((_, i) => "arg" + i))});\n");
            sb.Append("        }\n");
            sb.Append("        \n");
        }

        sb.Append("    }\n");
    }

    private static void AddRequestsWrapperStub(StringBuilder sb, List<Request> requests)
    {
        sb.Append("    public class RequestsWrapper(ProtocolObject protocolObject)\n");
        sb.Append("    {\n");
        foreach (var request in requests)
        {
            if (request.Arguments.Count > 0)
                sb.Append(
                    $"        public void {request.Name}(SocketConnection socketConnection, {string.Join(", ", request.Arguments.Select(arg => arg.Type + " " + arg.Name))})\n");
            else
                sb.Append($"        public void {request.Name}(SocketConnection socketConnection)\n");

            sb.Append("        {\n");
            sb.Append("            MessageWriter writer = new MessageWriter();\n");
            sb.Append("            writer.Write(protocolObject.Id);\n");
            sb.Append($"            writer.Write((int) RequestOpCode.{request.Name});\n");

            for (var i = 0; i < request.Arguments.Count; ++i)
            {
                var argument = request.Arguments[i];
                switch (argument.Type)
                {
                    case "int":
                    case "uint":
                    case "string":
                        sb.Append(
                            $"            writer.Write({argument.Name});\n");
                        break;
                    case "Fixed":
                    case "ObjectId":
                    case "NewId":
                        sb.Append(
                            $"            writer.Write({argument.Name}.Value);\n");
                        break;
                    default:
                        throw new Exception("Unknown type: " + argument.Type);
                }
            }

            sb.Append('\n');
            sb.Append("            byte[] data = writer.ToArray();\n");
            sb.Append("            int length = data.Length - 8;\n");
            sb.Append("            data[5] = (byte)(length >> 8);\n");
            sb.Append("            data[6] = (byte)(byte.MaxValue << 8 & length);\n");
            sb.Append('\n');
            sb.Append("            socketConnection.Write(data);\n");
            sb.Append("        }\n");
            sb.Append('\n');
        }

        sb.Append("    }\n");
    }
}