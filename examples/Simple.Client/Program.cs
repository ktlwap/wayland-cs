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

        NewId displayId = ProtocolObject.AllocateId();
        Display display = connection.Bind<Display>(displayId.Value, 1);
        
        NewId registryId = ProtocolObject.AllocateId();
        display.Requests.GetRegistry(registryId);
        Registry registry = connection.Bind<Registry>(registryId.Value, 1);

        registry.Events.Global += (uint name, string @interface, uint version) =>
        {
            Console.WriteLine($"Global: {name} {@interface} {version}");
            connection.Bind(@interface, registryId.Value, version);
        };
        
        NewId callbackId = ProtocolObject.AllocateId();
        connection.Bind(Callback.Name, callbackId.Value, 1);
        display.Requests.Sync(callbackId);

        connection.Callback!.Events.Done += (uint callbackData) =>
        {
            Console.WriteLine($"Request done: {callbackData}. Closing connection.");
            _isDone = true;
        };
        
        while (!_isDone)
            connection.EventQueue.Dispatch();
    }
}
