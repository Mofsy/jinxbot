using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [Serializable]
    public class ProfilePluginSettingConfiguration
    {
        [XmlAttribute]
        public string Name
        {
            get; set; 
        }

        [XmlElement(ElementName = "Value")]
        public string Value
        {
            get;
            set;
        }
    }
}
