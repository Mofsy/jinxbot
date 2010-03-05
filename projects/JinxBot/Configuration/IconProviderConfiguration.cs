using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [Serializable]
    public class IconProviderConfiguration 
    {
        [XmlAttribute("Name")]
        public string Name
        {
            get; set;
        }

        [XmlAttribute("Type")]
        public string TypeName
        {
            get; set; 
        }
    }
}
