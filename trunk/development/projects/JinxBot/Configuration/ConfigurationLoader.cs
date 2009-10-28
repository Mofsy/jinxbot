using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace JinxBot.Configuration
{
    internal static class ConfigurationLoader
    {
        private static string GetConfigFilePath()
        {
            string jinxBotAppDataPath = GetAppDataPath();

            string configFilePath = Path.Combine(jinxBotAppDataPath, "Settings.xml");
            return configFilePath;
        }

        private static string GetAppDataPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string jinxBotAppDataPath = Path.Combine(appDataPath, "JinxBot");
            if (!Directory.Exists(jinxBotAppDataPath))
                Directory.CreateDirectory(jinxBotAppDataPath);
            return jinxBotAppDataPath;
        }

        internal static JinxBotConfiguration LoadConfiguration()
        {
            using (FileStream fs = new FileStream(GetConfigFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                XDocument doc = XDocument.Load(sr);
                var root = doc.Root;
                if (root.Name != "JinxBotConfiguration")
                    throw new JinxBotConfigurationErrorException(JinxBotConfigurationError.CorruptConfigurationFile);
            }
        }

        

        private static JinxBotConfiguration LoadVersion10Configuration(TextReader reader)
        {
            
        }

        private static JinxBotConfiguration LoadPost10Configuration(TextReader reader, string version)
        {

        }
    }
}
