using Wayland.Client;
using Wayland.Protocol.Client;

namespace Simple.Client;

class Program
{
    private static bool _isDone = false;
    static void Main(string[] args)
    {
        using Connection connection = new Connection();
        Console.WriteLine("Connection established.");
        
        connection.Display.Requests.GetRegistry(ProtocolObject.AllocateId());

        connection.Registry.Events.Global += (uint name, string @interface, uint version) =>
        {
            Console.WriteLine($"Global: {name} {@interface} {version}");
        };
        
        connection.Callback.Events.Done += (uint callbackData) =>
        {
            Console.WriteLine($"Callback done: {callbackData}");
            _isDone = true;
        };
        
        connection.Display.Requests.Sync(ProtocolObject.AllocateId());

        while (!_isDone)
            connection.EventQueue.Dispatch();
        
        Console.WriteLine("Done round trip. Closing connection.");
    }
}
