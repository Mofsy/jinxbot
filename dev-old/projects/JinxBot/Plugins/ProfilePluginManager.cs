using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Views;
using BNSharp.BattleNet;
using JinxBot.Configuration;

namespace JinxBot.Plugins
{
    [Obsolete]
    internal class ProfilePluginManager
    {
        private ClientProfile m_profile;
        private BattleNetClient m_client;
        private List<ISingleClientPlugin> m_clientPlugins;
        private ProfileDocument m_view;
        private List<IMultiClientPlugin> m_multiClientPlugins;
        private Dictionary<ISingleClientPlugin, Dictionary<string, string>> m_clientPluginSettings;

        public ProfilePluginManager(ClientProfile profile, BattleNetClient client, ProfileDocument view)
        {
            m_profile = profile;
            m_client = client;
            m_view = view;

            m_clientPlugins = new List<ISingleClientPlugin>();
            m_multiClientPlugins = new List<IMultiClientPlugin>();
            m_clientPluginSettings = new Dictionary<ISingleClientPlugin, Dictionary<string, string>>();

            EnumeratePlugins(profile.PluginSettings);
        }

        private void EnumeratePlugins(ProfilePluginConfiguration[] profilePluginConfiguration)
        {
            
        }

        public void Close()
        {
            while (m_clientPlugins.Count > 0)
            {
                ISingleClientPlugin plugin = m_clientPlugins[0];
                plugin.DestroyPluginWindows(m_view);

                if (m_clientPluginSettings.ContainsKey(plugin))
                {
                    Dictionary<string, string> settings = m_clientPluginSettings[plugin];
                    plugin.Shutdown(settings);
                }
            }
        }
    }
}
