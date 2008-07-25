using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using BNSharp.Net;
using BNSharp;
using BNSharp.BattleNet;

namespace JinxBot.Views
{
    public partial class ProfileDocument : DockableDocument
    {
        private ChatDocument m_chat;
        private FriendsList m_friends;
        private ChannelList m_channel;
        private ClanList m_clan;
        private NewsList m_news;
        private BattleNetClient m_client;

        public ProfileDocument()
        {
            InitializeComponent();
        }

        public ProfileDocument(BattleNetClient client)
            : this()
        {
            m_client = client;
            this.Text = this.TabText = (client.Settings as ClientProfile).ProfileName;

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
        }

        /// <summary>
        /// Gets the client associated with this profile document.
        /// </summary>
        public BattleNetClient Client
        {
            get { return m_client; }
        }
    }
}
