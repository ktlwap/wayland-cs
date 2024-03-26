using System.Xml.Serialization;

namespace Wayland.Scanner.Types.Xml;

public class Request
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("since")]
    public uint Since { get; set; }

    [XmlElement("description")]
    public Description Description { get; set; }

    [XmlElement("arg")]
    public List<Argument> Arguments { get; set; }
}
