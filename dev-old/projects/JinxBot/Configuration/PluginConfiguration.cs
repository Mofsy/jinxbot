using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [Serializable]
    public class PluginConfiguration
    {
        [XmlElement("Assembly")]
        public string AssemblyName
        {
            get; set;
        }

        [XmlArray("Dependencies")]
        [XmlArrayItem("Dependency")]
        public DependencyConfiguration[] Dependencies
        {
            get; set; 
        }
    }
}
