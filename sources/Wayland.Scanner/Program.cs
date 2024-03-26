using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

class Program
{
    static void Main(string[] args)
    {
        Protocol protocol = XmlParser.Run("wayland.xml");
        
        Symbolizer.Process(protocol.Interfaces);
        CodeGenerator.Generate(protocol);
    }
}
