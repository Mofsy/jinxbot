using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Xml;
using BNSharp.BattleNet;

namespace JinxBot.Configuration
{
    /// <summary>
    /// Specifies the configuration settings for JinxBot.
    /// </summary>
    [Serializable]
    [XmlRoot("JinxBotConfiguration")]
    public class JinxBotConfiguration
    {
        /// <summary>
        /// Creates a new <see>JinxBotConfiguration</see>.
        /// </summary>
        /// <remarks>
        /// <para>This method supports the configuration infrastructure and is not intended to be used from your code.</para>
        /// </remarks>
        public JinxBotConfiguration()
        {
            this.Version = "1.0";

            this.Globals = new GlobalsConfigurationSection
                               {
                                   AllowDataCollection = false,
                                   IconProviders = new IconProviderConfiguration[]
                                                       {
                                                           new IconProviderConfiguration {Name = "BNI Icons", TypeName = "JinxBot.Views.Chat.BniIconProvider, JinxBot"},
                                                           new IconProviderConfiguration {Name = "Web Icons", TypeName = "JinxBot.Views.Chat.WebIconProvider, JinxBot"}
                                                       },
                                   KnownPlugins = new PluginConfiguration[0]
                               };
            this.Profiles = new ClientProfile[0];
        }

        [XmlAttribute("Version")]
        public string Version
        {
            get; set;
        }

        /// <summary>
        /// Gets the global settings object.
        /// </summary>
        /// <remarks>
        /// <para>Although setting this property is possible, that ability supports the configuration infrastructure and is not intended to be 
        /// used from your code.</para>
        /// </remarks>
        [XmlElement("Globals")]
        public GlobalsConfigurationSection Globals
        {
            get;
            set;
        }

        private ClientProfile[] m_profiles;
        /// <summary>
        /// Gets the profiles list.
        /// </summary>
        /// <remarks>
        /// <para>Although setting this property is possible, that ability supports the configuration infrastructure and is not intended to be 
        /// used from your code.</para>
        /// </remarks>
        [XmlArray(ElementName = "Profiles")]
        [XmlArrayItem("Profile")]
        public ClientProfile[] Profiles
        {
            get
            {
                if (object.ReferenceEquals(m_profiles, null))
                    m_profiles = new ClientProfile[0];
                return m_profiles;
            }
            set { m_profiles = value; }
        }

        private MultiClientPluginConfiguration[] m_mcpConfigs;

        [XmlArray(ElementName = "MultiClientPluginSettings")]
        [XmlArrayItem("MultiClientProfileConfiguration")]
        public MultiClientPluginConfiguration[] MultiClientPluginSettings
        {
            get
            {
                if (object.ReferenceEquals(m_mcpConfigs, null))
                    m_mcpConfigs = new MultiClientPluginConfiguration[0];
                return m_mcpConfigs;
            }
            set
            {
                m_mcpConfigs = value;
            }
        }

        public void AddMultiClientPluginConfiguration(MultiClientPluginConfiguration config)
        {
            List<MultiClientPluginConfiguration> configs = new List<MultiClientPluginConfiguration>(MultiClientPluginSettings);
            configs.Add(config);
            MultiClientPluginSettings = configs.ToArray();
        }

        public void RemoveMultiClientPluginConfiguration(MultiClientPluginConfiguration config)
        {
            var configs = new List<MultiClientPluginConfiguration>(MultiClientPluginSettings);
            configs.Remove(config);
            MultiClientPluginSettings = configs.ToArray();
        }

        /// <summary>
        /// Adds a profile to the configuration.
        /// </summary>
        /// <param name="profile">The profile to add.</param>
        public void AddProfile(ClientProfile profile)
        {
            List<ClientProfile> profiles = new List<ClientProfile>(Profiles);
            profiles.Add(profile);
            Profiles = profiles.ToArray();

            OnProfileAdded(EventArgs.Empty);
        }

        /// <summary>
        /// Removes a profile from the configuration.
        /// </summary>
        /// <param name="profile">The profile to remove.</param>
        public void RemoveProfile(ClientProfile profile)
        {
            List<ClientProfile> profiles = new List<ClientProfile>(Profiles);
            profiles.Remove(profile);
            Profiles = profiles.ToArray();

            OnProfileRemoved(EventArgs.Empty);
        }

        /// <summary>
        /// Saves the configuration file.
        /// </summary>
        public virtual void Save()
        {
            string configFilePath = GetConfigFilePath();
            XDocument doc = new XDocument();
            doc.Add(
                new XElement("JinxBotConfiguration", new XAttribute("Version", "1.1"),
                    new XElement("Globals",
                        new XComment(@"
    The Globals configuration section specifies the application-wide settings used
    by JinxBot.  It is included for the sake of adding support for app-wide data.
        "),
                        new XElement("AllowDataCollection", Globals.AllowDataCollection),
                        new XElement("KnownPlugins", 
                            from kp in Globals.KnownPlugins
                            select new XElement("Plugin",
                                new XElement("Assembly", kp.AssemblyName),
                                new XElement("Dependencies", 
                                    from d in kp.Dependencies
                                    select new XElement("Dependency", d.Filename)
                                    ) // </Dependencies>
                                ) // </Plugin>,
                            ), // </KnownPlugins>,
                            new XElement("IconProviders", 
                                new XComment(@"
        The IconProviders configuration section uses specific programming-based data to
        load and manage custom icons as displayed in the channel list or chat text.  It 
        should not be modified except by users who know what they are doing.
        "),
                                from ip in Globals.IconProviders
                                select new XElement("IconProvider", 
                                    new XAttribute("Name", ip.Name),
                                    new XAttribute("Type", ip.TypeName)
                                ) // </IconProvider>
                            ) // </IconProviders>
                        ), // </Globals>
                new XElement("Profiles",
                    from cp in Profiles
                    select new XElement("ClientProfile", 
                        new XAttribute("ProfileName", cp.ProfileName),
                        new XComment(" == Login information == "),
                        new XElement("Username", cp.Username),
                        new XElement("Password", cp.Password),
                        new XComment(" == Emulation information == "),
                        new XComment("Client should be one of: STAR, SEXP, D2DV, D2XP, W2BN, WAR3, W3XP"),
                        new XElement("Client", cp.Client),
                        new XComment("Version byte is a value commonly found on Battle.net client communities.  Please enter the *decimal* - not hex - version, so a version byte listed online as \"d3\", \"0xd3\", or \"&HD3\" should appear here as 211."),
                        new XElement("VersionByte", cp.VersionByte),
                        new XComment("GameExePath should be a fully-qualified path to one of: starcraft.exe, Warcraft II BNE.exe, Game.exe, or War3.exe"),
                        new XElement("GameExePath", cp.GameExe),
                        new XComment("StormDllPath should be a fully-qualified path to one of: Storm.dll or BnClient.dll"),
                        new XElement("StormDllPath", cp.GameFile2),
                        new XComment("BattleSnpPath should be a fully-qualified path to one of: Battle.snp, D2Client.snp, or Game.dll"),
                        new XElement("BattleSnpPath", cp.GameFile3),
                        new XComment("LockdownImagePath is used for Starcraft and Warcraft 2 version checking. It should be found on sites with game files, generally ending in \".bin\"."),
                        new XElement("LockdownImagePath", cp.ImageFile),
                        new XComment("Ping should be one of: Normal, MinusOneMs, ZeroMs, or ReplyBeforeVersioning.  ReplyBeforeVersioning is most accurate;"),
                        new XComment("Normal may cause a delay as JinxBot calculates your version check prior to responding to ping.  ZeroMs spoofs a ping of 0,"),
                        new XComment("and MinusOneMs spoofs a full lag bar (-1ms)."),
                        new XElement("Ping", cp.PingStyle.ToString()),
                        new XElement("CdKeyOwnerName", cp.CdKeyOwner),
                        new XElement("PrimaryCdKey", cp.CdKey1),
                        new XComment("SecondaryCdKey is only required for D2XP and W3XP clients."),
                        new XElement("SecondaryCdKey", cp.CdKey2),
                        new XComment(" == Server information == "),
                        new XComment("ServerUri and ServerPort are deprecated and should no longer be used."),
                        new XComment("Valid values of Gateway are: either a literal \"US East\", \"US West\", \"Asia\", or \"Europe\"; or"),
                        new XComment("a tree of descendants, including: Name, OldClientSuffix, Warcraft3ClientSuffix, ServerHost, and ServerPort"),
                        new XElement("Gateway", GetGatewayValue(cp)),
                        new XComment(" == Bot information == "),
                        new XElement("Trigger", cp.TriggerCharacter),
                        new XElement("VoidView", cp.VoidView),
                        new XElement("HomeChannel", cp.HomeChannel), 
                        new XElement("IncludeIconsInChat", cp.IncludeIconsInChat),
                        new XComment("IconStyle should correspond to the name of one of the IconProviders in the Globals section."),
                        new XElement("IconStyle", cp.IconProviderType),
                        new XComment(" Simulate should be set to true or false, and should only be used by developers. "),
                        new XElement("Simulate", cp.SimulateClient),
                        new XElement("PluginSettings",
                            new XComment("Plugin settings should only be managed by users who know what they're doing, or through the application."),
                            from ps in cp.PluginSettings
                            select new XElement("Plugin", 
                                new XElement("Name", ps.Name),
                                new XElement("Type", ps.Type),
                                new XElement("PluginAssembly", ps.Assembly),
                                new XElement("MultiClientName", ps.MultiClientName),
                                new XElement("Settings", 
                                    from set in ps.Settings
                                    select new XElement("Setting", 
                                        new XAttribute("Name", set.Name), 
                                        new XAttribute("Value", set.Value)
                                        ) )
                                    )
                                )            
                    ) // end <ClientProfile>
                ), // end <Profiles>
                new XElement("MultiClientPluginSettings",
                    new XComment("Plugin settings should only be managed by users who know what they're doing, or through the application."),
                    from mcps in this.MultiClientPluginSettings
                    select new XElement("MultiClientProfileConfiguration", new XAttribute("PluginName", mcps.PluginName),
                        new XElement("Settings",
                            from set in mcps.Settings
                            select new XElement("Setting", 
                                new XAttribute("Name", set.Name), 
                                new XAttribute("Value", set.Value)
                                ) )
                        )
                ) // end <MultiClientPluginSettings>
            )); // end <JinxBotConfiguration>
            using (FileStream fs = new FileStream(GetConfigFilePath(), FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            using (XmlTextWriter xtw = new XmlTextWriter(sw))
            {
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 4;
                xtw.IndentChar = ' ';

                doc.Save(xtw);
            }
        }

        private static object GetGatewayValue(ClientProfile cp)
        {
            Gateway g = cp.Gateway;
            Gateway[] predefined = new Gateway[] { Gateway.USEast, Gateway.USWest, Gateway.Asia, Gateway.Europe };
            Gateway match = predefined.Where(gw => gw.Name.Equals(g.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (match != null)
            {
                return match.Name;
            }
            else
            {
                return (new XElement[] { new XElement("Name", g.Name), new XElement("OldClientSuffix", g.OldClientSuffix), new XElement("Warcraft3ClientSuffix", g.Warcraft3ClientSuffix), new XElement("ServerHost", g.ServerHost), new XElement("ServerPort", g.ServerPort) }).AsEnumerable();
            }
        }

        /// <summary>
        /// Informs listeners that a profile was added to the collection.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ProfileAdded;
        /// <summary>
        /// Fires the <see>ProfileAdded</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnProfileAdded(EventArgs e)
        {
            if (ProfileAdded != null)
                ProfileAdded(this, e);
        }

        /// <summary>
        /// Informs listeners that a profile was removed from the collection.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ProfileRemoved;
        /// <summary>
        /// Fires the <see>ProfileRemoved</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnProfileRemoved(EventArgs e)
        {
            if (ProfileRemoved != null)
                ProfileRemoved(this, e);
        }

        private static JinxBotConfiguration s_instance = LoadConfig();


        private static JinxBotConfiguration LoadConfig()
        {
            return ConfigurationLoader.LoadConfiguration();
        }

        /// <summary>
        /// Gets the single instance of the configuration settings.
        /// </summary>
        public static JinxBotConfiguration Instance
        {
            get
            {
                return s_instance;
            }
        }

        /// <summary>
        /// Gets the path to which application data should be stored.
        /// </summary>
        public static string ApplicationDataPath
        {
            get { return GetAppDataPath(); }
        }

        /// <summary>
        /// Gets whether a configuration file exists.
        /// </summary>
        public static bool ConfigurationFileExists
        {
            get { return File.Exists(GetConfigFilePath()); }
        }

        private static string GetConfigFilePath()
        {
            string jinxBotAppDataPath = GetAppDataPath();

            string configFilePath = Path.Combine(jinxBotAppDataPath, "Settings.xml");
            return configFilePath;
        }

        internal static string GetAppDataPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string jinxBotAppDataPath = Path.Combine(appDataPath, "JinxBot");
            if (!Directory.Exists(jinxBotAppDataPath))
                Directory.CreateDirectory(jinxBotAppDataPath);
            return jinxBotAppDataPath;
        }
    }
}
