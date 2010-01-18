using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using BNSharp.Net;
using BNSharp;
using BNSharp.BattleNet;
using Timer = System.Timers.Timer;
using System.Timers;
using BNSharp.BattleNet.Stats;
using System.Globalization;

namespace JinxBot.Views
{
    internal partial class ChannelList : DockableToolWindow
    {
        private BattleNetClient m_client;
        private Timer m_voidViewTimer;
        private Dictionary<ChatUser, ToolTipPropertySet> m_tips;
        private static Dictionary<Product, Color> ProductTipColors = new Dictionary<Product, Color>
        {
            { Product.ChatClient, Color.DarkGray },
            { Product.Diablo2Expansion, Color.Gold },
            { Product.Diablo2Retail, Color.DarkRed },
            { Product.Diablo2Shareware, Color.DarkRed },
            { Product.DiabloRetail, Color.Orange },
            { Product.DiabloShareware, Color.Orange },
            { Product.JapanStarcraft, Color.OrangeRed },
            { Product.StarcraftBroodWar, Color.Maroon },
            { Product.StarcraftRetail, Color.Navy },
            { Product.StarcraftShareware, Color.Navy },
            { Product.UnknownProduct, Color.DarkGray },
            { Product.Warcraft2BNE, Color.DarkSlateBlue },
            { Product.Warcraft3Expansion, Color.SteelBlue },
            { Product.Warcraft3Retail, Color.DarkGreen }
        };
        private ProfileResourceProvider m_resourceProvider;

        public ChannelList()
        {
            InitializeComponent();

            m_voidViewTimer = new Timer(7500);
            m_voidViewTimer.Elapsed += new ElapsedEventHandler(m_voidViewTimer_Elapsed);
            m_voidViewTimer.AutoReset = true;

            m_tips = new Dictionary<ChatUser, ToolTipPropertySet>();
        }
        

        public ChannelList(BattleNetClient client)
            : this()
        {
            m_client = client;

            ProcessEventSetup();

            m_resourceProvider = ProfileResourceProvider.GetForClient(client);
        }

        private void ProcessEventSetup()
        {
            __UserJoined = new UserEventHandler(UserJoined);
            __UserLeft = new UserEventHandler(UserLeft);
            __UserShown = new UserEventHandler(UserShown);
            __JoinedChannel = new ServerChatEventHandler(JoinedChannel);
            __UserFlagsUpdated = new UserEventHandler(UserFlagsUpdated);
            __Disconnected = Disconnected;

            m_client.RegisterUserFlagsChangedNotification(Priority.Low, __UserFlagsUpdated);
            m_client.RegisterUserJoinedNotification(Priority.Low, __UserJoined);
            m_client.RegisterUserLeftNotification(Priority.Low, __UserLeft);
            m_client.RegisterUserShownNotification(Priority.Low, __UserShown);
            m_client.RegisterJoinedChannelNotification(Priority.Low, __JoinedChannel);
            m_client.RegisterDisconnectedNotification(Priority.Low, __Disconnected);
        }

        private EventHandler __Disconnected;
        void Disconnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Invokee(ClearChannelList));
            else
                ClearChannelList();
        }

        private ServerChatEventHandler __JoinedChannel;
        void JoinedChannel(object sender, ServerChatEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Invokee(ClearChannelList));
            else
                ClearChannelList();
        }

        private UserEventHandler __UserFlagsUpdated;
        void UserFlagsUpdated(object sender, UserEventArgs e)
        {
            if (m_voidView && "The Void".Equals(m_client.ChannelName, StringComparison.Ordinal)) /* void view */
            {
                if (InvokeRequired)
                    BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
                else
                    ShowUser(e);
            }
        }

        private void ClearChannelList()
        {
            this.listBox1.Items.Clear();
            UpdateUserCount();
        }

        private void UpdateUserCount()
        {
            this.Text = this.TabText = string.Format("{0} ({1} user{2})", m_client.ChannelName, listBox1.Items.Count, listBox1.Items.Count == 1 ? string.Empty : "s");
        }

        private UserEventHandler __UserShown;
        void UserShown(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
            else
                ShowUser(e);
        }

        private void ShowUser(UserEventArgs args)
        {
            ChatUser user = args.User;
            if (!listBox1.Items.Contains(args.User))
            {
                if ((user.Flags & UserFlags.ChannelOperator) == UserFlags.None)
                {
                    this.listBox1.Items.Add(user);
                }
                else
                {
                    int count = this.listBox1.Items.OfType<ChatUser>().Count(a => (a.Flags & UserFlags.ChannelOperator) == UserFlags.ChannelOperator);
                    this.listBox1.Items.Insert(count, user);
                }
            }
            UpdateUserCount();
        }

        private UserEventHandler __UserLeft;
        void UserLeft(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(RemoveUser), e);
            else
                RemoveUser(e);
        }

        private void RemoveUser(UserEventArgs args)
        {
            this.listBox1.Items.Remove(args.User);
            UpdateUserCount();
        }

        private UserEventHandler __UserJoined;
        void UserJoined(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
            else
                ShowUser(e);
        }

        private delegate void Invokee();
        private delegate void Invokee<T>(T args);

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                ChatUser user = listBox1.SelectedItem as ChatUser;
                if (user != null)
                {
                    if (user.Stats.Product == Product.Warcraft3Retail || user.Stats.Product == Product.Warcraft3Expansion)
                    {
                        m_client.RequestWarcraft3Profile(user);
                    }
                    else
                    {
                        UserProfileRequest request = new UserProfileRequest(user.Username,
                            UserProfileKey.Age, UserProfileKey.Sex, UserProfileKey.Location, UserProfileKey.Description,
                            UserProfileKey.AccountCreated, UserProfileKey.LastLogon, UserProfileKey.LastLogoff, UserProfileKey.TotalTimeLogged);

                        m_client.RequestUserProfile(user.Username, request);
                    }
                }
            }
        }

        void m_tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void listBox1_FilteringItem(object sender, JinxBot.Views.Chat.ItemFilteringEventArgs e)
        {
            ChatUser user = e.Item as ChatUser;
            if (user.Username.IndexOf(e.Filter, StringComparison.CurrentCultureIgnoreCase) == -1)
                e.ShouldBeRemoved = true;
        }

        private bool m_voidView;
        public bool VoidView
        {
            get
            {
                return m_voidView;
            }
            set
            {
                m_voidView = value;
                if (m_voidView)
                {
                    BeginVoidPolling();
                }
                else
                {
                    StopVoidPolling();
                    ClearChannelList();
                }
            }
        }

        
        private void StopVoidPolling()
        {
            m_voidViewTimer.Stop();
        }

        private void BeginVoidPolling()
        {
            m_voidViewTimer.Start();
            PollVoid();
        }

        private void PollVoid()
        {
            if (m_client.IsConnected &&
                m_client.ChannelName.Equals("The Void", StringComparison.Ordinal))
            {
                // construct username
                string username = DetermineClientUsername();
                m_client.SendMessage(string.Concat("/unignore ", username));
            }
        }

        private string DetermineClientUsername()
        {
            string username = m_client.Settings.Username;
            if (m_client.Settings.Client == Product.Diablo2Expansion.ProductCode ||
                m_client.Settings.Client == Product.Diablo2Retail.ProductCode ||
                m_client.Settings.Client == Product.Diablo2Shareware.ProductCode)
            {
                username = string.Concat("*", username);
            }
            return username;
        }

        void m_voidViewTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PollVoid();
        }

        private void ttChannelTip_Popup(object sender, PopupEventArgs e)
        {
            ToolTipPropertySet ttps = FindTip();

            if (ttps != null)
            {
                Size size = ttps.MeasureArrangement(listBox1.Font);
                e.ToolTipSize = size;
            }
        }

        private ToolTipPropertySet FindTip()
        {
            ToolTipPropertySet ttps = null;
            int index = this.listBox1.IndexFromPoint(listBox1.PointToClient(Cursor.Position));
            if (index < 65535 && index >= 0)
            {
                ChatUser user = listBox1.Items[index] as ChatUser;
                if (user != null)
                {
                    if (!m_tips.ContainsKey(user))
                        m_tips.Add(user, CreateTip(user));

                    ttps = m_tips[user];
                }
            }
            return ttps;
        }

        private ToolTipPropertySet CreateTip(ChatUser user)
        {
            // get the image to use
            Image icon = null;
            if (m_resourceProvider != null)
            {
                icon = m_resourceProvider.Icons.GetImageFor(user.Stats.Product);
            }

            // get the color to use.
            Color background = Color.Black;
            if (ProductTipColors.ContainsKey(user.Stats.Product))
                background = ProductTipColors[user.Stats.Product];
            
            // generate description
            StringBuilder description = new StringBuilder();
            description.Append("Product:    ");
            description.AppendLine(user.Stats.Product.Name);
            description.Append("Ping:       ");
            description.AppendLine(user.Ping.ToString(CultureInfo.InvariantCulture));
            description.Append("Flags:      ");
            description.AppendLine(user.Flags.ToString());
            description.AppendLine();

            if (user.Stats is StarcraftStats)
            {
                StarcraftStats stats = user.Stats as StarcraftStats;
                description.Append("Wins:       ");
                description.AppendLine(stats.Wins.ToString(CultureInfo.InvariantCulture));
                description.Append("Rank:       ");
                description.AppendLine(stats.LadderRank.ToString(CultureInfo.InvariantCulture));
                description.Append("Rating:     ");
                description.AppendLine(stats.LadderRating.ToString(CultureInfo.InvariantCulture));
            }
            else if (user.Stats is Diablo2Stats)
            {
                Diablo2Stats stats = user.Stats as Diablo2Stats;
                description.AppendLine(stats.IsExpansionCharacter ? "Expansion character" : "Classic character");
                if (stats.IsHardcoreCharacter)
                    description.Append("Hardcore ");
                if (stats.IsLadderCharacter)
                    description.Append("Ladder ");
                description.Append(stats.IsRealmCharacter ? "Realm " : "Open ");
                description.AppendLine("character");
                if (stats.IsRealmCharacter)
                {
                    description.AppendFormat(CultureInfo.InvariantCulture, "Level {0} {1}", stats.Level, stats.CharacterClass);
                    description.AppendLine();
                    description.AppendLine(stats.CharacterName);
                    description.AppendLine(stats.Difficulty.ToString());
                }
            }
            else if (user.Stats is Warcraft3Stats)
            {
                Warcraft3Stats stats = user.Stats as Warcraft3Stats;
                description.Append("Clan:       ");
                description.AppendLine(string.IsNullOrEmpty(stats.ClanTag) ? "(none)" : stats.ClanTag);
                description.AppendFormat(CultureInfo.InvariantCulture, "Level:      {0}", stats.Level);
                description.AppendLine();
            }

            ToolTipPropertySet ttps = new ToolTipPropertySet(user.Username, description.ToString(), background, icon, Color.White, background);
            return ttps;
        }

        private void ttChannelTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            ToolTipPropertySet ttps = FindTip();

            if (ttps != null)
            {
                ttps.Draw(e.Graphics, e.Bounds, e.Font);
            }
        }

        private int m_lastKnownIndex = -1;
        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = listBox1.PointToClient(Cursor.Position);
            int index = this.listBox1.IndexFromPoint(listBox1.PointToClient(Cursor.Position));
            if (m_lastKnownIndex != index)
            {
                m_lastKnownIndex = index;
                if (index < 65535 && index >= 0)
                {
                    this.ttChannelTip.SetToolTip(listBox1, (listBox1.Items[index] as ChatUser).Username);
                    //this.ttChannelTip.Show("User List", listBox1, point + new Size(5, 5), 5000);
                }
                else
                {
                    this.ttChannelTip.Hide(listBox1);
                }
            }
        }

        private void listBox1_MouseLeave(object sender, EventArgs e)
        {
            ttChannelTip.Hide(listBox1);
        }

        private void listBox1_MouseHover(object sender, EventArgs e)
        {

        }
    }
}
