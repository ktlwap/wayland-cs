using System.Diagnostics;
using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        stopwatch.Start();
        
        CodeGenerator.DeleteOldFiles();
        ClientCodeGenerator.DeleteOldFiles();
        
        Protocol waylandProtocol = XmlParser.Run("wayland.xml");
        Symbolizer.Process(waylandProtocol.Interfaces);
        CodeGenerator.Generate(waylandProtocol);
        ClientCodeGenerator.Generate(waylandProtocol);
        
        Protocol xdgShellProtocol = XmlParser.Run("xdg-shell.xml");
        Symbolizer.Process(xdgShellProtocol.Interfaces);
        CodeGenerator.Generate(xdgShellProtocol);
        ClientCodeGenerator.Generate(xdgShellProtocol);

        stopwatch.Stop();

        Console.WriteLine("Code generation completed in " + stopwatch.ElapsedMilliseconds + "ms.");
    }
}
