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
    public partial class ChannelList : DockableToolWindow
    {
        private BattleNetClient m_client;

        public ChannelList()
        {
            InitializeComponent();
        }

        public ChannelList(BattleNetClient client)
            : this()
        {
            m_client = client;

            ProcessEventSetup();
        }

        private void ProcessEventSetup()
        {
            __UserJoined = new UserEventHandler(UserJoined);
            __UserLeft = new UserEventHandler(UserLeft);
            __UserShown = new UserEventHandler(UserShown);
            __JoinedChannel = new ServerChatEventHandler(JoinedChannel);
            __UserFlagsUpdated = new UserEventHandler(UserFlagsUpdated);

            m_client.RegisterUserFlagsChangedNotification(Priority.Low, __UserFlagsUpdated);
            m_client.RegisterUserJoinedNotification(Priority.Low, __UserJoined);
            m_client.RegisterUserLeftNotification(Priority.Low, __UserLeft);
            m_client.RegisterUserShownNotification(Priority.Low, __UserShown);
            m_client.RegisterJoinedChannelNotification(Priority.Low, __JoinedChannel);
        }

        private ServerChatEventHandler __JoinedChannel;
        void JoinedChannel(object sender, ServerChatEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Invokee<ServerChatEventArgs>(ClearChannelList), e);
            else
                ClearChannelList(e);
        }

        private UserEventHandler __UserFlagsUpdated;
        void UserFlagsUpdated(object sender, UserEventArgs e)
        {
            if (m_client.ChannelName.Equals("The Void", StringComparison.Ordinal)) /* void view */
            {
                if (InvokeRequired)
                    BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
                else
                    ShowUser(e);
            }
        }

        private void ClearChannelList(ServerChatEventArgs e)
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

        private delegate void Invokee<T>(T args);

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                ChatUser user = listBox1.SelectedItem as ChatUser;
                if (user != null)
                {
                    UserProfileRequest request = new UserProfileRequest(user.Username,
                        UserProfileKey.Age, UserProfileKey.Sex, UserProfileKey.Location, UserProfileKey.Description,
                        UserProfileKey.AccountCreated, UserProfileKey.LastLogon, UserProfileKey.LastLogoff, UserProfileKey.TotalTimeLogged);

                    m_client.RequestUserProfile(user.Username, request);
                }
            }
        }
    }
}
