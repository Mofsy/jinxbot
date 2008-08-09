using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [XmlRoot("Globals")]
    [Serializable]
    public class GlobalsConfigurationSection
    {
        [XmlElement]
        public bool AllowDataCollection
        {
            get; set;
        }

        [XmlArray("KnownPlugins")]
        [XmlArrayItem("Plugin")]
        public PluginConfiguration[] KnownPlugins { get; set; }

        [XmlArray("IconProviders")]
        [XmlArrayItem("IconProvider")]
        public IconProviderConfiguration[] IconProviders
        {
            get; set; 
        }
    }
}
