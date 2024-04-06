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
                Display = new Display(Socket, id, version);
                break;
            case Registry.Name:
                Registry = new Registry(Socket, id, version);
                break;
            case Callback.Name:
                Callback = new Callback(Socket, id, version);
                break;
            case Compositor.Name:
                Compositor = new Compositor(Socket, id, version);
                break;
            case ShmPool.Name:
                ShmPool = new ShmPool(Socket, id, version);
                break;
            case Buffer.Name:
                Buffer = new Buffer(Socket, id, version);
                break;
            case DataSource.Name:
                DataSource = new DataSource(Socket, id, version);
                break;
            case DataDeviceManager.Name:
                DataDeviceManager = new DataDeviceManager(Socket, id, version);
                break;
            case Shell.Name:
                Shell = new Shell(Socket, id, version);
                break;
            case ShellSurface.Name:
                ShellSurface = new ShellSurface(Socket, id, version);
                break;
            case Surface.Name:
                Surface = new Surface(Socket, id, version);
                break;
            case Seat.Name:
                Seat = new Seat(Socket, id, version);
                break;
            case Pointer.Name:
                Pointer = new Pointer(Socket, id, version);
                break;
            case Keyboard.Name:
                Keyboard = new Keyboard(Socket, id, version);
                break;
            case Touch.Name:
                Touch = new Touch(Socket, id, version);
                break;
            case Output.Name:
                Output = new Output(Socket, id, version);
                break;
            case Region.Name:
                Region = new Region(Socket, id, version);
                break;
            case Subcompositor.Name:
                Subcompositor = new Subcompositor(Socket, id, version);
                break;
            case Subsurface.Name:
                Subsurface = new Subsurface(Socket, id, version);
                break;
            default:
                throw new NotImplementedException($"\"{name}\" is not implemented.");
        };
    }
}
