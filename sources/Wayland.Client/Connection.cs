using Wayland.Protocol.Client;
using Wayland.Protocol.Common;

namespace Wayland.Client;

public sealed partial class Connection : IDisposable
{
    private SocketConnection _socketConnection;
    
    public EventQueue EventQueue { get; }
    
    public Connection() : this(null) { }

    public Connection(string? name)
    {
        _socketConnection = new SocketConnection(FindWaylandUnixSocketPath(name));
        
        EventQueue = new EventQueue(_socketConnection);
    }
    
    public void Dispose()
    {
        _socketConnection.Dispose();
    }
    
    public T Bind<T>(uint id, uint version)
        where T : ProtocolObject
    {
        T? protocolObject = (T)Activator.CreateInstance(typeof(T), _socketConnection, id, version);
        if (protocolObject == null)
            throw new Exception("ProtocolType not found.");

        return protocolObject;
    }
    
    private static string FindWaylandUnixSocketPath(string? name)
    {
        string? xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        if (xdgRuntimeDir == null)
            throw new Exception("'XDG_RUNTIME_DIR' not set.");

        string path;
        if (name != null)
            path = Path.Join(xdgRuntimeDir, name);
        else
        {
            string? waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (waylandDisplay != null)
            {
                path = Path.Join(xdgRuntimeDir, waylandDisplay);
            }
            else
            {
                Console.WriteLine("No 'WAYLAND_DISPLAY' defined, using 'wayland-0' as fallback.");
                path = Path.Join(xdgRuntimeDir, "wayland-0");
            }
        }
        
        
        if (!File.Exists(path))
            throw new Exception($"Socket name '{path}' not found. Wayland server not running or listening on another Socket?");

        return path;
    }
}
