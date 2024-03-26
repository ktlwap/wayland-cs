using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Enum
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("since")]
    public uint Since { get; set; }

    [XmlAttribute("bitfield")]
    public bool Bitfield { get; set; }

    [XmlElement("description")]
    public Description Description { get; set; }

    [XmlElement("entry")]
    public List<Entry> Entries { get; set; }
}
