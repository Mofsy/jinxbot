using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins;
using BNSharp.BattleNet;
using JinxBot.Plugins.UI;
using JinxBot.Plugins.Data;
using JinxBot.Views;
using JinxBot.Plugins.BNSharp;

namespace JinxBot
{
    internal sealed class JinxBotClient : IJinxBotClient
    {
        private BattleNetClient m_client;
        private ProfileDocument m_window;
        private ProfileResourceProvider m_resourceProvider;
        private IJinxBotDatabase m_database;
        private ClientProfile m_profile;

        public JinxBotClient(ClientProfile profile)
        {
            m_client = new BattleNetClient(profile);
            m_profile = profile;
            m_resourceProvider = ProfileResourceProvider.RegisterProvider(m_client);
            m_window = new ProfileDocument(m_client);

            bool hasSetCommandQueue = false;
            
            // initialize plugins

            if (!hasSetCommandQueue)
            {
                m_client.CommandQueue = new TimedMessageQueue();
            }

            if (m_database == null)
                m_database = new JinxBotDefaultDatabase();
        }

        #region IJinxBotClient Members

        public BattleNetClient Client
        {
            get { return m_client; }
        }

        public ProfileDocument ProfileDocument
        {
            get { return m_window; }
        }

        public IChatTab MainWindow
        {
            get { return m_window; }
        }

        public IJinxBotDatabase Database
        {
            get { return m_database; }
        }

        public void SendMessage(string message)
        {
            m_client.SendMessage(message);
        }

        #endregion
    }
}
