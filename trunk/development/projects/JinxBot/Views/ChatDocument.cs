using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.Net;
using JinxBot.Controls.Docking;
using BNSharp;
using JinxBot.Controls;
using System.Globalization;
using Tmr = System.Timers.Timer;
using BNSharp.BattleNet.Stats;
using System.Threading;
using JinxBot.Views.Chat;
using BNSharp.BattleNet;

namespace JinxBot.Views
{
    public partial class ChatDocument : DockableDocument
    {
        private BattleNetClient m_client;
        private string m_userName;
        private bool m_inChat;
        private DateTime m_enteredChat;
        private ProfileResourceProvider m_prp;

        private Tmr m_tmr;

        /// <summary>
        /// Initializes a new <see>Chatdocument</see> with no client.
        /// </summary>
        /// <remarks>
        /// <para>This is provided as a constructor for subclassing that does not want to use the default event registration setup.  When using the
        /// <see>ChatDocument(BattleNetClient)</see> constructor, client events are registered automatically at 
        /// <see cref="Priority">Low priority</see>.</para>
        /// </remarks>
        protected ChatDocument()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new <see>ChatDocument</see> to handle the specified client.
        /// </summary>
        /// <param name="client">The client to handle.</param>
        public ChatDocument(BattleNetClient client)
            : this()
        {
            m_client = client;
            m_prp = ProfileResourceProvider.GetForClient(client);

            SetupEventRegistration();

            m_tmr = new Tmr();
            m_tmr.Interval = 185000.0;
            m_tmr.Elapsed += new System.Timers.ElapsedEventHandler(m_tmr_Elapsed);
            m_tmr.SynchronizingObject = this;
        }

        void m_tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan ts = DateTime.Now - m_enteredChat;
            m_client.SendMessage(string.Format(CultureInfo.CurrentCulture, "/me is boticulating.  Bot power. <anti-idle; uptime {0}d {1}h {2}m {3}s>.  :: the new jinxbot ::", (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds));
        }

        private void SetupEventRegistration()
        {
            m_client.ChannelDidNotExist += new ServerChatEventHandler(m_client_ChannelDidNotExist);
            m_client.ChannelListReceived += new ChannelListEventHandler(m_client_ChannelListReceived);
            m_client.ChannelWasFull += new ServerChatEventHandler(m_client_ChannelWasFull);
            m_client.ChannelWasRestricted += new ServerChatEventHandler(m_client_ChannelWasRestricted);
            m_client.ClientCheckFailed += new ClientCheckFailedEventHandler(m_client_ClientCheckFailed);
            m_client.ClientCheckPassed += new EventHandler(m_client_ClientCheckPassed);
            m_client.CommandSent += new InformationEventHandler(m_client_CommandSent);
            m_client.Connected += new EventHandler(m_client_Connected);
            m_client.Disconnected += new EventHandler(m_client_Disconnected);
            m_client.EnteredChat += new EnteredChatEventHandler(m_client_EnteredChat);
            m_client.Error += new ErrorEventHandler(m_client_Error);
            m_client.Information += new InformationEventHandler(m_client_Information);
            m_client.InformationReceived += new ServerChatEventHandler(m_client_InformationReceived);
            m_client.JoinedChannel += new ServerChatEventHandler(m_client_JoinedChannel);
            m_client.LoginFailed += new EventHandler(m_client_LoginFailed);
            m_client.LoginSucceeded += new EventHandler(m_client_LoginSucceeded);
            m_client.MessageSent += new ChatMessageEventHandler(m_client_MessageSent);
            m_client.ServerBroadcast += new ServerChatEventHandler(m_client_ServerBroadcast);
            m_client.ServerErrorReceived += new ServerChatEventHandler(m_client_ServerErrorReceived);
            m_client.UserEmoted += new ChatMessageEventHandler(m_client_UserEmoted);
            m_client.UserJoined += new UserEventHandler(m_client_UserJoined);
            m_client.UserLeft += new UserEventHandler(m_client_UserLeft);
            m_client.UserShown += new UserEventHandler(m_client_UserShown);
            m_client.UserSpoke += new ChatMessageEventHandler(m_client_UserSpoke);
            m_client.WardentUnhandled += new EventHandler(m_client_WardentUnhandled);
            m_client.WhisperReceived += new ChatMessageEventHandler(m_client_WhisperReceived);
            m_client.WhisperSent += new ChatMessageEventHandler(m_client_WhisperSent);
        }

        void m_client_UserShown(object sender, UserEventArgs e)
        {
            AnnounceUser(e);
        }

        void m_client_WhisperSent(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("You whisper to ", Color.Magenta), new ChatNode(e.Username, Color.Magenta), new ChatNode(": ", Color.Magenta), new ChatNode(e.Text, Color.Magenta));
        }

        void m_client_WhisperReceived(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Username, Color.Magenta), new ChatNode(" whispers: ", Color.Magenta), new ChatNode(e.Text, Color.Magenta));
        }

        void m_client_WardentUnhandled(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("WARNING: Warden was requested but unhandled.  You may be disconnected.", Color.IndianRed));
        }

        void m_client_UserSpoke(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("[", Color.LightBlue), new ChatNode(e.Username, Color.White), new ChatNode("]: ", Color.LightBlue), new ChatNode(e.Text, Color.White));
        }

        void m_client_UserLeft(object sender, UserEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Username, Color.Lime), new ChatNode(" left the channel.", Color.Yellow));
        }

        void m_client_UserJoined(object sender, UserEventArgs e)
        {
            AnnounceUser(e);
        }

        private void AnnounceUser(UserEventArgs e)
        {
            UserStats us = UserStats.Parse(e.Username, e.StatsData);
            string imgID = m_prp.Icons.GetImageIdFor(e.Flags, us);
            ImageChatNode productNode = new ImageChatNode(string.Concat(imgID, ".jpg"),
                m_prp.Icons.GetImageFor(e.Flags, us), m_prp.Icons.GetImageIdFor(e.Flags, us));

            switch (us.Product.ProductCode)
            {
                case "DRTL":
                case "DSHR":
                case "SSHR":
                case "STAR":
                case "SEXP":
                case "JSTR":
                case "W2BN":
                    StarcraftStats ss = us as StarcraftStats;
                    if (ss == null)
                        chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode,
                            new ChatNode(string.Format(" joined the channel with {0}.", e.Statstring), Color.Yellow));
                    else
                    {
                        chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0} ({1} win{2}, ladder rating {3}, rank {4}).", ss.Product.Name, ss.Wins, ss.Wins != 1 ? "s" : string.Empty,
                            ss.LadderRating, ss.LadderRank), Color.Yellow));
                    }
                    break;
                case "D2DV":
                case "D2XP":
                    Diablo2Stats ds = us as Diablo2Stats;
                    if (ds == null)
                        chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0}.", us.Product.Name), Color.Yellow));
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" joined the channel with ");
                        sb.Append(ds.Product.Name);
                        if (ds.IsRealmCharacter)
                        {
                            sb.Append(" as ");
                            sb.Append(ds.CharacterName);
                            sb.AppendFormat(", a level {0}", ds.Level);
                            if (ds.IsHardcoreCharacter)
                                sb.Append(" hardcore");
                            if (ds.IsLadderCharacter)
                                sb.Append(" ladder");
                            if (ds.IsExpansionCharacter)
                                sb.Append(" Expansion ");
                            else
                                sb.Append(" Classic ");
                            sb.Append(ds.CharacterClass);
                            sb.Append(".");

                            chat.AddChat(new ChatNode(ds.UserName, Color.Lime), new ChatNode(sb.ToString(), Color.Yellow));
                        }
                        else
                        {
                            chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0}.", us.Product.Name), Color.Yellow));
                        }
                    }
                    break;
                case "WAR3":
                case "W3XP":
                    Warcraft3Stats ws = us as Warcraft3Stats;
                    if (ws == null)
                        chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0}.", e.Statstring), Color.Yellow));
                    else
                    {
                        if (string.IsNullOrEmpty(ws.ClanTag))
                        {
                            chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0}, {1} icon tier {2}, level {3}.", ws.Product.Name, ws.IconRace, ws.IconTier, ws.Level), Color.Yellow));
                        }
                        else
                        {
                            chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" of clan {0} joined the channel with {1}, {2} icon tier {3}, level {4}.", ws.ClanTag, ws.Product.Name, ws.IconRace, ws.IconTier, ws.Level), Color.Yellow));
                        }
                    }
                    break;
                default:
                    chat.AddChat(new ChatNode(e.Username, Color.Lime), productNode, new ChatNode(string.Format(" joined the channel with {0} ({1}).", us.Product.Name, e.Statstring), Color.Yellow));
                    break;

            }
        }

        void m_client_UserEmoted(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("<", Color.Yellow), new ChatNode(e.Username, Color.White), new ChatNode(string.Concat(" ", e.Text, ">"), Color.Yellow));
        }

        void m_client_ServerErrorReceived(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("[Error]: ", Color.Gray), new ChatNode(e.Text, Color.Orange));
        }

        void m_client_ServerBroadcast(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("[Server]: ", Color.Gray), new ChatNode(e.Text, Color.Gray));
        }

        void m_client_MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (m_inChat)
            {
                //if (e.EventType == ChatEventType.Emote)
                //{
                //    chat.AddChat(new ChatNode("<", Color.Yellow), new ChatNode(m_userName, Color.White), new ChatNode(string.Concat(" ", e.Text, ">"), Color.Yellow));
                //}
                //else
                //{
                    chat.AddChat(new ChatNode("[", Color.LightBlue), new ChatNode(m_userName, Color.White), new ChatNode("]: ", Color.LightBlue), new ChatNode(e.Text, Color.White));
                //}
            }
        }

        void m_client_LoginSucceeded(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Login succeeded!", Color.LimeGreen));
        }

        void m_client_LoginFailed(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Login failed.", Color.Red));
        }

        void m_client_JoinedChannel(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("Joined Channel: ", Color.Yellow), new ChatNode(e.Text, Color.White));
            ChannelFlags flags = (ChannelFlags)e.Flags;
            if ((flags & ChannelFlags.SilentChannel) == ChannelFlags.SilentChannel)
                chat.AddChat(new ChatNode("This is a silent channel.", Color.LightBlue));

            ThreadStart ts = delegate
            {
                this.Text = e.Text;
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();

            m_tmr.Stop();
            m_tmr.Start();
        }

        void m_client_InformationReceived(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("[Server]: ", Color.Gray), new ChatNode(e.Text, Color.Gray));
        }

        void m_client_Information(object sender, InformationEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Information, Color.LightSteelBlue));
        }

        void m_client_Error(object sender, ErrorEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Error, Color.Orange));
            if (e.IsDisconnecting)
            {
                chat.AddChat(new ChatNode("This error is causing you to disconnect.", Color.OrangeRed));
            }
        }

        void m_client_EnteredChat(object sender, EnteredChatEventArgs e)
        {
            Product clientProduct = Product.GetByProductCode(m_client.Settings.Client.ToUpperInvariant());
            string imgID = m_prp.Icons.GetImageIdFor(UserFlags.None, UserStats.CreateDefault(clientProduct));
            Image userImg = ProfileResourceProvider.GetForClient(m_client).Icons.GetImageFor(UserFlags.None, UserStats.CreateDefault(clientProduct));
            chat.AddChat(new ChatNode("Entered chat as ", Color.Yellow),
                new ImageChatNode(string.Concat(imgID, ".jpg"), 
                    userImg, clientProduct.Name),
                new ChatNode(e.UniqueUsername, Color.White));
            m_userName = e.UniqueUsername;
            m_inChat = true;
            m_enteredChat = DateTime.Now;
        }

        void m_client_Disconnected(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Disconnected.", Color.Yellow));
            m_inChat = false;
            m_userName = null;

            m_tmr.Stop();
        }

        void m_client_Connected(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Connected!", Color.Yellow));
        }

        void m_client_CommandSent(object sender, InformationEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Information, Color.Teal));
        }

        void m_client_ClientCheckPassed(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Versioning passed!", Color.LimeGreen));
        }

        void m_client_ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            chat.AddChat(new ChatNode(string.Format(CultureInfo.CurrentCulture, "Versioning check failed; {0}", e.Reason), Color.OrangeRed));
        }

        void m_client_ChannelWasRestricted(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }

        void m_client_ChannelWasFull(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }

        void m_client_ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            List<ChatNode> nodesToAdd = new List<ChatNode>();
            nodesToAdd.Add(new ChatNode("Available Channels:", Color.LightSteelBlue));
            foreach (string s in e.Channels)
            {
                nodesToAdd.Add(ChatNode.NewLine);
                nodesToAdd.Add(new ChatNode(" - ", Color.Yellow));
                nodesToAdd.Add(new ChatNode(s, Color.White));
            }
            chat.AddChat(nodesToAdd);
        }

        void m_client_ChannelDidNotExist(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }

        private void chat_MessageReady(object sender, MessageEventArgs e)
        {
            if (m_inChat)
            {
                if (e.Message.Length < 224)
                {
                    m_client.SendMessage(e.Message);
                }
            }
            else
            {
                chat.AddChat(new ChatNode("You are not yet in chat!", Color.YellowGreen));
            }
        }
    }
}
