using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Configuration;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace JinxBot.Plugins
{
    internal static class PluginFactory
    {
        private static Dictionary<string, Assembly> m_loadedAssemblies;
        private static Dictionary<string, PluginInfo> m_pluginTypes;
        private static Dictionary<string, IMultiClientPlugin> m_activeMultiClientPlugins;
        private static object m_lock = new object();

        static PluginFactory()
        {
            m_loadedAssemblies = new Dictionary<string, Assembly>();
            m_pluginTypes = new Dictionary<string, PluginInfo>();
            m_activeMultiClientPlugins = new Dictionary<string, IMultiClientPlugin>();
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
                if (info.SupportsMultiClient)
                {
                    return GetMultiClientPlugin(info);
                }
                else
                {
                    return GetRegularPlugin(info);
                }
            }
        }

        private static IJinxBotPlugin GetRegularPlugin(PluginInfo info)
        {
            Debug.Assert(info != null);

            return null;
        }

        private static IJinxBotPlugin GetMultiClientPlugin(PluginInfo info)
        {
            throw new NotImplementedException();
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
                m_pluginTypes.Add(pluginConfiguration.Name, info);
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
