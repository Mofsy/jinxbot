using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Diagnostics;
using BNSharp.BattleNet;

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

                string version = "1.0";
                var ver = root.Attribute("Version");
                if (ver != null) version = ver.Value;

                fs.Seek(0, SeekOrigin.Begin);

                if (version == "1.0")
                    return LoadVersion10Configuration(sr);
                else
                    return LoadPost10Configuration(doc, version);
            }
        }

        

        private static JinxBotConfiguration LoadVersion10Configuration(TextReader reader)
        {
            XmlSerializer xs = new XmlSerializer(typeof(JinxBotConfiguration));
            try
            {
                JinxBotConfiguration config = xs.Deserialize(reader) as JinxBotConfiguration;
                return config;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new JinxBotConfiguration();
            }
        }

        private static JinxBotConfiguration LoadPost10Configuration(XDocument document, string version)
        {
            return new JinxBotConfiguration
            {
                Globals = GlobalsConfigurationSection.Load(document.Element("Globals")),
                Version = version,
                Profiles = document.Element("Profiles").Elements("ClientProfile").As(
                    cp => new ClientProfile
                    {
                        Client = cp.Property("Client").Value,
                        VersionByte = cp.Property("VersionByte").AsInt32(),
                        CdKey1 = cp.Property("PrimaryCdKey").Value,
                        CdKey2 = cp.Property("SecondaryCdKey").Value,
                        GameExe = cp.Property("GameExePath").Value,
                        GameFile2 = cp.Property("StormDllPath").Value,
                        GameFile3 = cp.Property("BattleSnpPath").Value,
                        ImageFile = cp.Property("LockdownImagePath").Value,
                        Username = cp.Property("Username").Value,
                        Password = cp.Property("Password").Value,
                        Gateway = DetermineGateway(cp.Property("Gateway"), cp.Property("ServerUri"), cp.Property("ServerPort")),
                        CdKeyOwner = cp.Property("CdKeyOwner").Value,
                        PingStyle = (PingStyle)Enum.Parse(typeof(PingStyle), cp.Property("Ping").Value, true),
                        HomeChannel = cp.Property("HomeChannel").Value,
                        IncludeIconsInChat = cp.Property("IncludeIconsInChat").AsBool(),
                        ProfileName = cp.Property("ProfileName").Value,
                        TriggerCharacter = cp.Property("Trigger").Value,
                        VoidView = cp.Property("VoidView").AsBool(),
                        IconProviderType = cp.Property("IconStyle").Value,
                        PluginSettings = cp.Element("PluginSettings").Elements("Plugin").As(
                            ps => new ProfilePluginConfiguration
                            {
                                Assembly = ps.Property("PluginAssembly").Value,
                                Name = ps.Property("Name").Value,
                                Type = ps.Property("Type").Value,
                                MultiClientName = ps.Property("MultiClientName").Value,
                                Settings = ps.Element("Settings").Elements("Setting").As(
                                    set => new ProfilePluginSettingConfiguration
                                    {
                                        Name = set.Property("Name").Value,
                                        Value = set.Property("Value").Value
                                    }).ToArray(),
                            }).ToArray()
                    }).ToArray(),
                MultiClientPluginSettings = document.Element("MultiClientPluginSettings").Elements("MultiClientProfileConfiguration").As(
                    mcp => new MultiClientPluginConfiguration
                    {
                        PluginName = mcp.Property("PluginName").Value,
                        Settings = mcp.Element("Settings").Elements("Setting").As(
                            set => new ProfilePluginSettingConfiguration
                                {
                                    Name = set.Property("Name").Value,
                                    Value = set.Property("Value").Value
                                }).ToArray()
                    }).ToArray()
            };
        }

        private static Gateway DetermineGateway(XProperty gateway, XProperty serverUri, XProperty serverPort)
        {
            Gateway[] gateways = new Gateway[] { Gateway.Asia, Gateway.Europe, Gateway.USEast, Gateway.USWest };

            if (gateway == null)
            {
                string server = serverUri.Value;
                int port = serverUri.AsInt32();
                var realGateway = gateways.Where(g => g.ServerHost.Equals(server, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (realGateway != null)
                {
                    return realGateway;
                }
                else
                {
                    return new Gateway(server, "", "", server, port);
                }
            }
            else
            {
                XElement element = gateway.AsElement;
                if (element.HasElements)
                {
                    return new Gateway(
                        element.Property("Name").Value,
                        element.Property("OldClientSuffix").Value,
                        element.Property("Warcraft3ClientSuffix").Value,
                        element.Property("ServerHost").Value,
                        element.Property("ServerPort").AsInt32()
                        );
                }
                else
                {
                    var realGateway = gateways.Where(g => g.Name.Equals(gateway.Value, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (realGateway != null)
                        return realGateway;

                    return new Gateway(gateway.Value, "", "", gateway.Value, 6112);
                }
            }
        }
    }
}
