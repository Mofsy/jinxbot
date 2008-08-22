using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

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
        public void Save()
        {
            string configFilePath = GetConfigFilePath();
            using (FileStream fs = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(GetType());
                    xs.Serialize(fs, this);
                    fs.Flush();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                
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
            string configFilePath = GetConfigFilePath();
            if (!File.Exists(configFilePath))
            {
                return new JinxBotConfiguration();
            }
            else
            {
                using (FileStream fs = new FileStream(configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(JinxBotConfiguration));
                    try
                    {
                        JinxBotConfiguration config = xs.Deserialize(fs) as JinxBotConfiguration;
                        return config;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        return new JinxBotConfiguration();
                    }
                }
            }
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

        private static string GetAppDataPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string jinxBotAppDataPath = Path.Combine(appDataPath, "JinxBot");
            if (!Directory.Exists(jinxBotAppDataPath))
                Directory.CreateDirectory(jinxBotAppDataPath);
            return jinxBotAppDataPath;
        }
    }
}
