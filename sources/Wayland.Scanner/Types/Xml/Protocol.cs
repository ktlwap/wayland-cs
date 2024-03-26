using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

[XmlRoot("protocol")]
public class Protocol
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("copyright")]
    public Copyright Copyright { get; set; }

    [XmlElement("description")]
    public Description Description { get; set; }

    [XmlElement("interface")]
    public List<Interface> Interfaces { get; set; }
}
