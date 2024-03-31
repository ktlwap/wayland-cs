using Wayland.Protocol.Client;

namespace Wayland.Client;

public sealed partial class Connection
{
    public Display? Display { get; private set; }
    public Registry? Registry { get; private set; }
    public Callback? Callback { get; private set; }
    
    public void Bind(string name, uint id, uint version)
    {
        switch (name)
        {
            case Display.Name:
                Display = new Display(_socketConnection, id, version);
                break;
            case Registry.Name:
                Registry = new Registry(_socketConnection, id, version);
                break;
            case Callback.Name:
                Callback = new Callback(_socketConnection, id, version);
                break;
        };
    }
}
