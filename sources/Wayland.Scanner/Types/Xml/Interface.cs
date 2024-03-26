using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Interface
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("version")]
    public uint Version { get; set; }

    [XmlElement("description")]
    public Description Description { get; set; }

    [XmlElement("request")]
    public List<Request> Requests { get; set; }

    [XmlElement("event")]
    public List<Event> Events { get; set; }

    [XmlElement("enum")]
    public List<Enum> Enums { get; set; }
}
