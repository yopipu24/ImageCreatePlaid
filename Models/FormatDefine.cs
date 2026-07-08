namespace ImageCreatePlaid.Models
{
    public class FormatDefine
    {
        [System.Xml.Serialization.XmlAttribute("id")]
        public string Id { get; set; }

        [System.Xml.Serialization.XmlElement("width")]
        public int Width { get; set; }

        [System.Xml.Serialization.XmlElement("height")]
        public int Height { get; set; }
    }
}
