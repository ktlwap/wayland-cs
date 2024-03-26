using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Copyright
{
    [XmlText]
    public string Content { get; set; }
}
