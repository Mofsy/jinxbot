using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{

    public class MultiClientPluginConfiguration
    {
        [XmlAttribute]
        public string PluginName
        {
            get;
            set;
        }

        [XmlArray("Settings")]
        [XmlArrayItem("Setting")]
        public ProfilePluginSettingConfiguration[] Settings
        {
            get;
            set;
        }
    }
}
