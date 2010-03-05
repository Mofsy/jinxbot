using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Configuration;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using JinxBot.Plugins.UI;

namespace JinxBot.Plugins
{
    internal static class PluginFactory
    {
        private static Dictionary<string, Assembly> m_loadedAssemblies;
        private static Dictionary<string, PluginInfo> m_pluginTypes;
        private static Dictionary<string, IMultiClientPlugin> m_activeMultiClientPlugins;
        private static Dictionary<IMultiClientPlugin, int> m_instanceCounter;
        private static Dictionary<IMultiClientPlugin, string> m_instancesToNames;

        private static object m_lock = new object();

        internal static IMainWindow MainWindow
        {
            get;
            set;
        }

        static PluginFactory()
        {
            m_loadedAssemblies = new Dictionary<string, Assembly>();
            m_pluginTypes = new Dictionary<string, PluginInfo>();
            m_activeMultiClientPlugins = new Dictionary<string, IMultiClientPlugin>();
            m_instanceCounter = new Dictionary<IMultiClientPlugin, int>();
            m_instancesToNames = new Dictionary<IMultiClientPlugin, string>();
        }

        public static IJinxBotPlugin CreatePlugin(ProfilePluginConfiguration pluginConfiguration)
        {
            lock (m_lock)
            {
                if (!m_loadedAssemblies.ContainsKey(pluginConfiguration.Assembly))
                    if (!LoadAssembly(pluginConfiguration))
                        return null;

                if (!m_pluginTypes.ContainsKey(pluginConfiguration.Type))
                    if (!LoadType(pluginConfiguration))
                        return null;

                PluginInfo info = m_pluginTypes[pluginConfiguration.Type];
                IJinxBotPlugin plugin = null;
                if (info.SupportsMultiClient && !string.IsNullOrEmpty(pluginConfiguration.MultiClientName))
                {
                    if (m_activeMultiClientPlugins.ContainsKey(pluginConfiguration.MultiClientName))
                    {
                        plugin = m_activeMultiClientPlugins[pluginConfiguration.MultiClientName];
                        IMultiClientPlugin mcp = plugin as IMultiClientPlugin;
                        m_instanceCounter[mcp] = m_instanceCounter[mcp] + 1;
                    }
                    else
                    {
                        plugin = Instantiate(info, pluginConfiguration);
                        IMultiClientPlugin mcp = plugin as IMultiClientPlugin;
                        if (mcp == null)
                            throw new InvalidCastException("Plugin \"" + pluginConfiguration.Name + "\" is configured as a multi-client plugin but does not support that interface.");
                        mcp.CreatePluginWindows(MainWindow);
                        m_activeMultiClientPlugins.Add(pluginConfiguration.MultiClientName, mcp);
                        m_instancesToNames.Add(mcp, pluginConfiguration.MultiClientName);
                        m_instanceCounter.Add(mcp, 1);
                    }
                }
                else
                {
                    plugin = Instantiate(info, pluginConfiguration);
                }

                return plugin;
            }
        }

        private static IJinxBotPlugin Instantiate(PluginInfo info, ProfilePluginConfiguration config)
        {
            IJinxBotPlugin plugin = Activator.CreateInstance(info.Type) as IJinxBotPlugin;
            if (plugin == null)
                throw new InvalidCastException("Could not cast \"" + info.Type.FullName + "\" to \"IJinxBotPlugin\".");

            Dictionary<string, string> settings = config.Settings.ToDictionary(s => s.Name, s => s.Value);
            plugin.Startup(settings);

            return plugin;
        }

        public static void ClosePluginInstance(ProfilePluginConfiguration config, IJinxBotPlugin pluginInstance)
        {
            lock (m_lock)
            {
                SavePluginInstanceConfig(config, pluginInstance);

                IMultiClientPlugin mcp = pluginInstance as IMultiClientPlugin;
                if (mcp != null)
                {
                    int count = m_instanceCounter[mcp];
                    if (count == 1)
                    {
                        // this is the last instance, so shut it down completely
                        m_instanceCounter.Remove(mcp);
                        mcp.DestroyPluginWindows(MainWindow);
                        string name = m_instancesToNames[mcp];
                        m_instancesToNames.Remove(mcp);
                        m_activeMultiClientPlugins.Remove(name);
                    }
                    else
                    {
                        m_instanceCounter[mcp] = count - 1;
                    }
                }
            }
        }

        private static void SavePluginInstanceConfig(ProfilePluginConfiguration config, IJinxBotPlugin pluginInstance)
        {
            Dictionary<string, string> settings = config.Settings.ToDictionary(s => s.Name, s => s.Value);
            pluginInstance.Shutdown(settings);
            List<ProfilePluginSettingConfiguration> settingsToSave = new List<ProfilePluginSettingConfiguration>();
            foreach (string key in settings.Keys)
            {
                settingsToSave.Add(new ProfilePluginSettingConfiguration { Name = key, Value = settings[key] });
            }
            config.Settings = settingsToSave.ToArray();
        }

        private static bool LoadType(ProfilePluginConfiguration pluginConfiguration)
        {
            Assembly asm = m_loadedAssemblies[pluginConfiguration.Assembly];
            Type type = asm.GetType(pluginConfiguration.Type, false, false);

            if (type != null)
            {
                JinxBotPluginAttribute[] attr = type.GetCustomAttributes(typeof(JinxBotPluginAttribute), false) as JinxBotPluginAttribute[];
                if (attr.Length == 0)
                    return false;

                PluginInfo info = new PluginInfo(attr[0], type);
                m_pluginTypes.Add(pluginConfiguration.Type, info);
                return true;
            }

            return false;
        }

        private static bool LoadAssembly(ProfilePluginConfiguration pluginConfiguration)
        {
            string assemblyPath = Path.Combine(GetPluginsStoragePath(), pluginConfiguration.Assembly);
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException("The specified plugin was not found.");

            try
            {
                Assembly asm = Assembly.LoadFile(assemblyPath);
                m_loadedAssemblies.Add(pluginConfiguration.Assembly, asm);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetPluginsStoragePath()
        {
            string jinxbotAppDataPath = GetAppDataPath();
            string pluginsStoragePath = Path.Combine(jinxbotAppDataPath, "Plugins");
            if (!Directory.Exists(pluginsStoragePath))
                Directory.CreateDirectory(pluginsStoragePath);

            return pluginsStoragePath;
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
