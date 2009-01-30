using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;

namespace JinxBot.Plugins
{
    internal class ProfilePluginManager
    {
        private ClientProfile m_profile;
        private BattleNetClient m_client;

        public ProfilePluginManager(ClientProfile profileConfiguration, BattleNetClient client)
        {
            m_client = client;
            m_profile = profileConfiguration;
        }
    }
}
