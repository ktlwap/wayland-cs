using System.Diagnostics;
using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        stopwatch.Start();
        
        Protocol protocol = XmlParser.Run("wayland.xml");
        Symbolizer.Process(protocol.Interfaces);
        CodeGenerator.Generate(protocol);
        ClientCodeGenerator.Generate(protocol);
        
        stopwatch.Stop();

        Console.WriteLine("Code generation completed in " + stopwatch.ElapsedMilliseconds + "ms.");
    }
}
