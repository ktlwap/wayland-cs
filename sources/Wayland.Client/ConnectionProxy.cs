using Wayland.Protocol.Client;

namespace Wayland.Client;

public sealed partial class Connection
{
    public Display Display { get; private set; }
    public Registry? Registry { get; private set; }
    public Callback? Callback { get; private set; }
}
