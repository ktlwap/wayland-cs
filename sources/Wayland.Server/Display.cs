namespace Wayland.Server;

public sealed class Display
{
    private static string GetNextAvailableUnixSocketPath(string? name)
    {
        string? xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        if (xdgRuntimeDir == null)
            throw new Exception("'XDG_RUNTIME_DIR' not set.");

        if (name != null)
        {
            string path = $"{xdgRuntimeDir}/{name}";
            if (File.Exists(path))
                throw new Exception($"Socket name '{path}' already in use. Consider using a different socket.");

            return path;
        }
        else
        {
            string[] filenames = Directory.GetFiles(xdgRuntimeDir, "wayland-*");

            int availableSocketNumber = 0;
            foreach (string filename in filenames)
            {
                if (int.TryParse(filename.Replace("wayland-", ""), out int result))
                {
                    if (result >= availableSocketNumber)
                        availableSocketNumber = result + 1;
                }
            }

            return xdgRuntimeDir + "/wayland-" + availableSocketNumber;
        }
    }
}
