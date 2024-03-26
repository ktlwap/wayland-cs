using Wayland.Client;

namespace Simple.Client;

class Program
{
    static void Main(string[] args)
    {
        using Connection connection = new Connection();
        Console.WriteLine("Connection established.");
    }
}
