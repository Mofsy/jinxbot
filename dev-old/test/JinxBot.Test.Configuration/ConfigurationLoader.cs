using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    public static class ConfigurationLoader
    {
        public static void SerializeDefaultConfig()
        {
            XmlSerializer ser = new XmlSerializer(typeof(JinxBotConfiguration));
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                ser.Serialize(sw, JinxBotConfiguration.Instance);
            }

            Debug.WriteLine(sb.ToString());
        }
    }
}
