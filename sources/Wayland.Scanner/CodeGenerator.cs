using System.Text;
using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

public static class CodeGenerator
{
    private const string ClientFilePath = "../../../../Wayland.Protocol.Client/";
    private const string ServerFilePath = "../../../../Wayland.Protocol.Server/";

    public static void Generate(Protocol protocol)
    {
        string[] filenames = Directory.GetFiles(ServerFilePath);
        if (!filenames.Any(f =>
                f.EndsWith("Wayland.Protocol.Server.csproj") || f.EndsWith("Wayland.Protocol.Client.csproj")))
            throw new Exception("Output directory is not correct. Please set it to the correct path.");


        foreach (string filename in filenames)
        {
            if (filename.EndsWith(".gen.cs"))
                File.Delete(filename);
        }

        foreach (Interface @interface in protocol.Interfaces)
        {
            if (@interface.Name is "Shm" or "DataOffer")
                // These are not supported yet due to fd as argument.
                continue;

            using FileStream fileStream = File.Create(Path.Combine(ServerFilePath, @interface.Name + ".gen.cs"));

            byte[] source = GenerateSource(@interface);
            fileStream.Write(source, 0, source.Length);
        }
    }

    private static byte[] GenerateSource(Interface @interface)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("namespace Wayland.Protocol.Common\n");
        sb.Append('\n');
        sb.Append($"public sealed class {@interface.Name} : ProtocolObject\n");
        sb.Append("{\n");
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
        sb.Append("    public enum RequestOpCode : ushort\n");
        sb.Append("    {\n");
        for (int i = 0; i < events.Count; i++)
        {
            sb.Append($"        {events[i].Name} = {i},\n");
        }

        sb.Append("    }\n");
    }

    private static void AddRequestOpCodeStub(StringBuilder sb, List<Request> requests)
    {
        sb.Append("    public enum RequestOpCode : ushort\n");
        sb.Append("    {\n");
        for (int i = 0; i < requests.Count; i++)
        {
            sb.Append($"        {requests[i].Name} = {i},\n");
        }

        sb.Append("    }\n");
    }

    private static void AddEventsWrapperStub(StringBuilder sb, List<Event> events)
    {
        sb.Append("    public class EventsWrapper(ProtocolObject protocolObject)\n");
        sb.Append("    {\n");
        foreach (Event @event in events)
        {
            sb.Append(
                $"        public Action<{string.Join(", ", @event.Arguments.Select(arg => arg.Type))}>? {@event.Name} {{ get; set; }}\n");
        }

        sb.Append("        \n");
        sb.Append("        internal void HandleEvent(SocketConnection socketConnection)\n");
        sb.Append("        {\n");
        sb.Append("            ushort length = socketConnection.ReadUInt16();\n");
        sb.Append("            ushort opCode = socketConnection.ReadUInt16();\n");
        sb.Append("            \n");
        sb.Append("            switch (opCode)\n");
        sb.Append("            {\n");

        foreach (Event @event in events)
        {
            sb.Append($"                case (ushort) EventOpCode.{@event.Name}:\n");
            sb.Append($"                    Handle{@event.Name}Event(socketConnection, length);\n");
            sb.Append("                    return;\n");
        }

        sb.Append("            }\n");
        sb.Append("        }\n");
        sb.Append("        \n");

        foreach (Event @event in events)
        {
            sb.Append(
                $"        private void Handle{@event.Name}Event(SocketConnection socketConnection, ushort length)\n");
            sb.Append("        {\n");
            sb.Append("        }\n");
            sb.Append("        \n");
        }

        sb.Append("    }\n");
    }

    private static void AddRequestsWrapperStub(StringBuilder sb, List<Request> requests)
    {
        sb.Append("    public class RequestsWrapper(ProtocolObject protocolObject)\n");
        sb.Append("    {\n");
        foreach (Request request in requests)
        {
            if (request.Arguments.Count > 0)
                sb.Append(
                    $"        public void {request.Name}(SocketConnection socketConnection, {string.Join(", ", request.Arguments.Select(arg => arg.Type + " " + arg.Name))})\n");
            else
                sb.Append($"        public void {request.Name}(SocketConnection socketConnection)\n");

            sb.Append("        {\n");
            sb.Append("            byte[] buffer = new byte[length / 8];\n");
            sb.Append("            socketConnection.Read(buffer, 0, buffer.Length);\n");
            sb.Append('\n');
            sb.Append("            int index = 0;\n");

            for (int i = 0; i < request.Arguments.Count; ++i)
            {
                Argument argument = request.Arguments[i];
                switch (argument.Type)
                {
                    case "int":
                        sb.Append(
                            $"            int arg{i} = (int)(buffer[index++] << 8 & buffer[index++]);\n");
                        break;
                    case "uint":
                        sb.Append(
                            $"            uint arg{i} = (uint)(buffer[index++] << 8 & buffer[index++]);\n");
                        break;
                    case "Fixed":
                        sb.Append(
                            $"            uint arg{i} = new Fixed((int)buffer[index++] << 8 & buffer[index++]);\n");
                        break;
                    case "NewId":
                        sb.Append(
                            $"            uint arg{i} = new NewId((uint)buffer[index++] << 8 & buffer[index++]);\n");
                        break;
                    case "ObjectId":
                        sb.Append(
                            $"            uint arg{i} = new ObjectId((uint)buffer[index++] << 8 & buffer[index++]);\n");
                        break;
                    case "string":
                        sb.Append(
                            $"            uint arg{i} = StringConverter.Parse(buffer, in index, out index);\n");
                        break;
                    default:
                        throw new Exception("Unknown type: " + argument.Type);
                }
            }

            sb.Append('\n');
            sb.Append(
                $"            {request.Name}?.Invoke({string.Join(", ", request.Arguments.Select(arg => arg.Name))});\n");
            sb.Append("        }\n");
            sb.Append('\n');
        }

        sb.Append("    }\n");
    }
}