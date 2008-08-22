using System;
using System.Collections.Generic;
using JinxBot.Controls.Docking;
using BNSharp.Net;
using BNSharp;
using BNSharp.BattleNet;
using JinxBot.Plugins;
using JinxBot.Plugins.UI;
using JinxBot.Reliability;
using System.Diagnostics;

namespace JinxBot.Views
{
    public partial class ProfileDocument : DockableDocument, IChatTab
    {
        private ChatDocument m_chat;
        private FriendsList m_friends;
        private ChannelList m_channel;
        private ClanList m_clan;
        private NewsList m_news;
        private BattleNetClient m_client;
        private Dictionary<string, int> m_assemblyNamesToErrors;
        private List<DockableDocument> m_documents;

        public ProfileDocument()
        {
            InitializeComponent();
            m_assemblyNamesToErrors = new Dictionary<string, int>();
            m_documents = new List<DockableDocument>();
        }

        public ProfileDocument(BattleNetClient client)
            : this()
        {
            m_client = client;
            this.Text = this.TabText = (client.Settings as ClientProfile).ProfileName;

            if (client.Settings.Client == Product.StarcraftRetail.ProductCode || client.Settings.Client == Product.StarcraftBroodWar.ProductCode)
            {
                WardenModule module = new WardenModule(client);
                m_client.WardenHandler = module;
            }

            client.EventExceptionThrown += new EventExceptionEventHandler(client_EventExceptionThrown);

            m_chat = new ChatDocument(client);
            m_chat.Text = "Main chat window (Disconnected)";
            m_chat.Show(this.dock);

            m_channel = new ChannelList(client);
            m_channel.Show(this.dock);

            IBattleNetSettings settings = client.Settings;
            string clientCode = settings.Client;
            if (clientCode.Equals(Product.Warcraft3Retail.ProductCode, StringComparison.Ordinal) ||
                clientCode.Equals(Product.Warcraft3Expansion.ProductCode, StringComparison.Ordinal))
            {
                m_friends = new FriendsList(client);
                m_friends.Show(this.dock);

                m_clan = new ClanList(client);
                m_clan.Show(this.dock);

                m_news = new NewsList(client);
                m_news.Show(this.dock);
                m_news.DockState = DockState.DockLeftAutoHide;
            }
            else if (clientCode.Equals(Product.StarcraftRetail.ProductCode, StringComparison.Ordinal) ||
                clientCode.Equals(Product.StarcraftBroodWar.ProductCode, StringComparison.Ordinal))
            {
                m_friends = new FriendsList(client);
                m_friends.Show(this.dock);
            }

            m_channel.Show();

            m_ssUri = m_chat.StylesheetUri;

            m_documents.Add(m_chat);
        }

        void client_EventExceptionThrown(object sender, EventExceptionEventArgs e)
        {
            GlobalErrorHandler.ReportEventException(this.Text, e);
            string assemblyFullName = e.FaultingMethod.Method.DeclaringType.Assembly.FullName;
            if (m_assemblyNamesToErrors.ContainsKey(assemblyFullName))
            {
                m_assemblyNamesToErrors[assemblyFullName]++;
            }
            else
            {
                m_assemblyNamesToErrors.Add(assemblyFullName, 1);
            }
        }

        /// <summary>
        /// Gets the client associated with this profile document.
        /// </summary>
        public BattleNetClient Client
        {
            get { return m_client; }
        }

        private Uri m_ssUri;
        public Uri StylesheetUri
        {
            get { return m_ssUri; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                m_ssUri = value;
                foreach (DockableDocument doc in m_documents)
                {
                    IChatTab tab = doc as IChatTab;
                    if (tab != null)
                        tab.StylesheetUri = value;
                }
            }
        }
    }
}
