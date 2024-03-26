using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Description
{
    [XmlAttribute("summary")]
    public string Summary { get; set; }

    [XmlText]
    public string Content { get; set; }
}
