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
using JinxBot.Configuration;
using BNSharp.Plugins;
using JinxBot.Util;
using JinxBot.Plugins.Script;

namespace JinxBot
{
    internal sealed class JinxBotClient : IJinxBotClient
    {
        private BattleNetClient m_client;
        private ProfileDocument m_window;
        private ProfileResourceProvider m_resourceProvider;
        private IJinxBotDatabase m_database;
        private ClientProfile m_profile;
        private Dictionary<ProfilePluginConfiguration, IJinxBotPlugin> m_activePlugins;
        private CommandTranslator m_cmdTranslator;

        private List<ICommandHandler> m_commandHandlers;

        public JinxBotClient(ClientProfile profile)
        {
            m_activePlugins = new Dictionary<ProfilePluginConfiguration, IJinxBotPlugin>();

            if (profile.SimulateClient)
                m_client = new SimulatedBattleNetClient(profile);
            else
                m_client = new BattleNetClient(profile);

            m_profile = profile;
            m_resourceProvider = ProfileResourceProvider.RegisterProvider(m_client);
            m_cmdTranslator = new CommandTranslator(this);

            bool hasSetCommandQueue = false;

            if (m_database == null)
                m_database = new JinxBotDefaultDatabase();

            // finally, initialize ui
            m_window = new ProfileDocument(this);

            // initialize plugins
            m_commandHandlers = new List<ICommandHandler>();
            foreach (ProfilePluginConfiguration pluginConfig in profile.PluginSettings)
            {
                hasSetCommandQueue = ProcessPlugin(hasSetCommandQueue, pluginConfig);
            }

            ProfilePluginConfiguration jsConfig = new ProfilePluginConfiguration
            {
                Assembly = "JinxBot.Plugins.Script.dll",
                Name = "JavaScript Plugin",
                Settings = new ProfilePluginSettingConfiguration[0],
                Type = "JinxBot.Plugins.Script.JinxBotJavaScriptPlugin"
            };
            hasSetCommandQueue = ProcessPlugin(hasSetCommandQueue, jsConfig);

            if (!hasSetCommandQueue)
            {
                m_client.CommandQueue = new TimedMessageQueue();
            }
        }

        private bool ProcessPlugin(bool hasSetCommandQueue, ProfilePluginConfiguration pluginConfig)
        {
            IJinxBotPlugin plugin = PluginFactory.CreatePlugin(pluginConfig);
            m_activePlugins.Add(pluginConfig, plugin);

            // test if the plugin is a command queue
            ICommandQueue commandQueuePlugin = plugin as ICommandQueue;
            if (!hasSetCommandQueue && commandQueuePlugin != null)
            {
                m_client.CommandQueue = commandQueuePlugin;
                hasSetCommandQueue = true;
            }

            // test if the plugin is a database plugin
            IJinxBotDatabase databasePlugin = plugin as IJinxBotDatabase;
            if (databasePlugin != null)
                m_database = databasePlugin;

            // test if the plugin is a command handler
            ICommandHandler handler = plugin as ICommandHandler;
            if (handler != null)
                m_commandHandlers.Add(handler);

            // test if the plugin is multi-client
            IMultiClientPlugin mcp = plugin as IMultiClientPlugin;
            if (mcp != null)
                mcp.AddClient(this);
            else
            {
                ISingleClientPlugin scp = plugin as ISingleClientPlugin;
                if (scp != null)
                {
                    scp.CreatePluginWindows(this.ProfileDocument);
                    scp.RegisterEvents(this);
                }
            }

            return hasSetCommandQueue;
        }

        public void Close()
        {
            m_client.Close();
            
            m_cmdTranslator.Dispose();
            m_cmdTranslator = null;

            foreach (ProfilePluginConfiguration config in m_activePlugins.Keys)
            {
                IJinxBotPlugin plugin = m_activePlugins[config];
                IMultiClientPlugin mcp = plugin as IMultiClientPlugin;
                if (mcp != null)
                    mcp.RemoveClient(this);

                ISingleClientPlugin scp = plugin as ISingleClientPlugin;
                if (scp != null)
                {
                    scp.UnregisterEvents(this);
                    scp.DestroyPluginWindows(this.ProfileDocument);
                }

                PluginFactory.ClosePluginInstance(config, plugin);

                JinxBotConfiguration.Instance.Save();
            }


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

        internal IEnumerable<ICommandHandler> CommandHandlers
        {
            get
            {
                foreach (ICommandHandler handler in m_commandHandlers)
                    yield return handler;
            }
        }

        internal ClientProfile Settings
        {
            get { return m_profile; }
        }
        #endregion
    }
}
