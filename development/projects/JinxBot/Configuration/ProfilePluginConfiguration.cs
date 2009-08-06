using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [Serializable]
    public class ProfilePluginConfiguration
    {
        [XmlElement("Name")]
        public string Name
        {
            get; set; 
        }

        [XmlElement("Assembly")]
        public string Assembly
        {
            get; set; 
        }

        [XmlElement("Type")]
        public string Type
        {
            get;
            set;
        }

        [XmlElement("MultiClientName", IsNullable = true)]
        public string MultiClientName
        {
            get;
            set;
        }

        [XmlArray("Settings")]
        [XmlArrayItem("Setting")]
        public ProfilePluginSettingConfiguration[] Settings
        {
            get; set; 
        }
    }
}
