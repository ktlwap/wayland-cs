using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Argument
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("summary")]
    public string Summary { get; set; }

    [XmlAttribute("interface")]
    public string Interface { get; set; }

    [XmlAttribute("allow_null")]
    public bool AllowNull { get; set; }

    [XmlAttribute("enum")]
    public string Enum { get; set; }
}
