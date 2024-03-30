using Wayland.Client;
using Wayland.Protocol.Client;
using Wayland.Protocol.Common;

namespace Simple.Client;

class Program
{
    private static bool _isDone = false;
    static void Main(string[] args)
    {
        using Connection connection = new Connection();
        Console.WriteLine("Connection established.");

        NewId registryId = ProtocolObject.AllocateId();
        connection.Display.Requests.GetRegistry(ProtocolObject.AllocateId());
        connection.Bind(Registry.Name, registryId.Value, 1);

        connection.Registry!.Events.Global += (uint name, string @interface, uint version) =>
        {
            Console.WriteLine($"Global: {name} {@interface} {version}");
            connection.Bind(@interface, registryId.Value, version);
        };
        
        connection.Display.Requests.Sync(ProtocolObject.AllocateId());
        
        connection.Callback.Events.Done += (uint callbackData) =>
        {
            Console.WriteLine($"Request done: {callbackData}. Closing connection.");
            _isDone = true;
        };

        while (!_isDone)
            connection.EventQueue.Dispatch();
    }
}
