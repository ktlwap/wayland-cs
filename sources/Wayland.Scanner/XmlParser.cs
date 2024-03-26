using System.Xml.Serialization;
using Wayland.Scanner.Types.Xml;

namespace Wayland.Scanner;

public static class XmlParser
{
    public static Protocol Run(string filename)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Protocol));
        using (FileStream reader = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            return (Protocol) serializer.Deserialize(reader);
        }
    }
}
