using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    [Serializable]
    public class DependencyConfiguration
    {
        [XmlText]
        public string Filename
        {
            get; set;
        }
    }
}
