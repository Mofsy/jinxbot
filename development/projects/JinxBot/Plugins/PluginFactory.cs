using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins
{
    internal static class PluginFactory
    {
        private static Dictionary<string, PluginInfo> m_plugins;

        private static IJinxBotPlugin Instantiate(string pluginName)
        {
            if (!m_plugins.ContainsKey(pluginName))
                throw new ArgumentOutOfRangeException("pluginName", pluginName, "The specified plugin name is not registered.");

            PluginInfo info = m_plugins[pluginName];
            IJinxBotPlugin pluginInstance = Activator.CreateInstance(info.Type) as IJinxBotPlugin;

            return pluginInstance;
        }
    }
}
