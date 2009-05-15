using System;
using System.Collections.Generic;
using JinxBot.Controls;
using JinxBot.Controls.Docking;
using BNSharp.Net;
using BNSharp;
using BNSharp.BattleNet;
using JinxBot.Plugins;
using JinxBot.Plugins.UI;
using JinxBot.Reliability;
using System.Diagnostics;
using BNSharp.BattleNet.Stats;
using JinxBot.Configuration;

using JinxBot.Views.Chat;
using System.Threading;

namespace JinxBot.Views
{
    public partial class ProfileDocument : DockableDocument, IChatTab, IProfileDocument
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

            if (client.Settings.Client == Product.StarcraftRetail.ProductCode || client.Settings.Client == Product.StarcraftBroodWar.ProductCode 
                || client.Settings.Client == Product.Warcraft3Retail.ProductCode || client.Settings.Client == Product.Warcraft3Expansion.ProductCode)
            {
                //WardenPacketHandler module = new WardenPacketHandler(client);
                //m_client.WardenHandler = module;
            }

            //m_client.ProxyConnector = new JinxBot.Plugins.BNSharp.Socks4ProxyConnector(client.Settings as ClientProfile);

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

            client.RegisterWarcraftProfileReceivedNotification(Priority.Low, WarcraftProfileReceived);

            m_channel.VoidView = this.VoidView;
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

        public bool VoidView
        {
            get
            {
                ClientProfile profile = m_client.Settings as ClientProfile;
                return profile.VoidView;
            }
            set
            {
                ClientProfile profile = m_client.Settings as ClientProfile;
                profile.VoidView = value;
                JinxBotConfiguration.Instance.Save();
                m_channel.VoidView = value;
            }
        }

        private Thread m_connectingThread;
        public void BeginConnecting()
        {
            m_chat.AddChat(new ChatNode("Connecting...", CssClasses.Connected));
            m_connectingThread = new Thread(__Connect);
            m_connectingThread.IsBackground = true;
            m_connectingThread.Start();
        }

        private void __Connect()
        {
            try
            {
                m_client.Connect();
            }
            catch (ThreadAbortException)
            {
            }
        }

        public void Disconnect()
        {
            if (m_connectingThread.IsAlive)
            {
                m_connectingThread.Abort();
            }
            m_client.Close();
        }

        private void WarcraftProfileReceived(object sender, WarcraftProfileEventArgs e)
        {
            ThreadStart ts = delegate
            {
                WarcraftProfileDisplayDocument doc = new WarcraftProfileDisplayDocument(e);
                doc.ShowDialog(this);
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }

        protected override void OnFormClosed(System.Windows.Forms.FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            m_client.Dispose();
        }

        #region IProfileDocument Members

        public void AddDocument(DockableDocument profileDocument)
        {
            if (profileDocument == null)
                throw new ArgumentNullException("profileDocument");

            profileDocument.Show(dock);
        }

        public void RemoveDocument(DockableDocument profileDocument)
        {
            profileDocument.Hide();
        }

        public void AddToolWindow(DockableToolWindow toolWindow)
        {
            toolWindow.Show(dock);
        }

        public void RemoveToolWindow(DockableToolWindow toolWindow)
        {
            toolWindow.Hide();
        }

        #endregion
    }
}
