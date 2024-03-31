using Wayland.Protocol.Client;
using Buffer = Wayland.Protocol.Client.Buffer;

namespace Wayland.Client;

public sealed partial class Connection
{
    public Display? Display { get; private set; }
    public Registry? Registry { get; private set; }
    public Callback? Callback { get; private set; }
    public Compositor? Compositor { get; private set; }
    public ShmPool? ShmPool { get; private set; }
    public Buffer? Buffer { get; private set; }
    public DataSource? DataSource { get; private set; }
    public DataDeviceManager? DataDeviceManager { get; private set; }
    public Shell? Shell { get; private set; }
    public ShellSurface? ShellSurface { get; private set; }
    public Surface? Surface { get; private set; }
    public Seat? Seat { get; private set; }
    public Pointer? Pointer { get; private set; }
    public Keyboard? Keyboard { get; private set; }
    public Touch? Touch { get; private set; }
    public Output? Output { get; private set; }
    public Region? Region { get; private set; }
    public Subcompositor? Subcompositor { get; private set; }
    public Subsurface? Subsurface { get; private set; }
    
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
            case Compositor.Name:
                Compositor = new Compositor(_socketConnection, id, version);
                break;
            case ShmPool.Name:
                ShmPool = new ShmPool(_socketConnection, id, version);
                break;
            case Buffer.Name:
                Buffer = new Buffer(_socketConnection, id, version);
                break;
            case DataSource.Name:
                DataSource = new DataSource(_socketConnection, id, version);
                break;
            case DataDeviceManager.Name:
                DataDeviceManager = new DataDeviceManager(_socketConnection, id, version);
                break;
            case Shell.Name:
                Shell = new Shell(_socketConnection, id, version);
                break;
            case ShellSurface.Name:
                ShellSurface = new ShellSurface(_socketConnection, id, version);
                break;
            case Surface.Name:
                Surface = new Surface(_socketConnection, id, version);
                break;
            case Seat.Name:
                Seat = new Seat(_socketConnection, id, version);
                break;
            case Pointer.Name:
                Pointer = new Pointer(_socketConnection, id, version);
                break;
            case Keyboard.Name:
                Keyboard = new Keyboard(_socketConnection, id, version);
                break;
            case Touch.Name:
                Touch = new Touch(_socketConnection, id, version);
                break;
            case Output.Name:
                Output = new Output(_socketConnection, id, version);
                break;
            case Region.Name:
                Region = new Region(_socketConnection, id, version);
                break;
            case Subcompositor.Name:
                Subcompositor = new Subcompositor(_socketConnection, id, version);
                break;
            case Subsurface.Name:
                Subsurface = new Subsurface(_socketConnection, id, version);
                break;
            default:
                throw new NotImplementedException($"\"{name}\" is not implemented.");
        };
    }
}
