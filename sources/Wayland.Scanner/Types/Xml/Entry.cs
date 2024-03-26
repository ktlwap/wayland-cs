using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Entry
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }

    [XmlAttribute("summary")]
    public string Summary { get; set; }

    [XmlAttribute("since")]
    public uint Since { get; set; }
}
