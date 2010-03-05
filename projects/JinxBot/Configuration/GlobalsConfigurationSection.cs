using System;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Linq;

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

        internal static GlobalsConfigurationSection Load(XElement globals)
        {
            return new GlobalsConfigurationSection
            {
                AllowDataCollection = globals.Element("AllowDataCollection").AsBool(),
                IconProviders = globals.Element("IconProviders").Elements("IconProvider").As<IconProviderConfiguration>(
                    x => new IconProviderConfiguration
                    {
                        Name = x.Property("Name").Value,
                        TypeName = x.Property("Type").Value
                    }
                ).ToArray(),
                KnownPlugins = globals.Element("KnownPlugins").Elements("Plugin").As<PluginConfiguration>(
                    x => new PluginConfiguration
                    {
                        AssemblyName = x.Property("Assembly").Value,
                        Dependencies = x.Element("Dependencies").Elements("Dependency").As(
                            d => new DependencyConfiguration
                            {
                                Filename = d.Value
                            }).ToArray()
                    }
                ).ToArray()
            };
        }
    }
}
