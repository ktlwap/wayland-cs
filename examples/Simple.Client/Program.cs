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
        connection.Bind(Display.Name, displayId.Value, 1);
        
        NewId registryId = ProtocolObject.AllocateId();
        connection.Bind(Registry.Name, registryId.Value, 1);
        connection.Display.Requests.GetRegistry(registryId);

        connection.Registry!.Events.Global += (uint name, string @interface, uint version) =>
        {
            Console.WriteLine($"Global: {name} {@interface} {version}");
            connection.Bind(@interface, registryId.Value, version);
        };
        
        NewId callbackId = ProtocolObject.AllocateId();
        connection.Bind(Callback.Name, callbackId.Value, 1);
        connection.Display.Requests.Sync(callbackId);

        connection.Callback!.Events.Done += (uint callbackData) =>
        {
            Console.WriteLine($"Request done: {callbackData}. Closing connection.");
            _isDone = true;
        };

        connection.Socket.Flush();
        while (!_isDone)
            connection.EventQueue.Dispatch();
    }
}
