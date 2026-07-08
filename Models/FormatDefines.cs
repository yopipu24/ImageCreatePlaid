namespace ImageCreatePlaid.Models
{
    [System.Xml.Serialization.XmlRoot("formatdefines")]
    public class FormatDefines
    {
        [System.Xml.Serialization.XmlElement("formatdefine")]
        public List<FormatDefine> Defines { get; set; }
    }
}
