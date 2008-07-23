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
            m_client.UserJoined += new UserEventHandler(m_client_UserJoined);
            m_client.UserLeft += new UserEventHandler(m_client_UserLeft);
            m_client.UserShown += new UserEventHandler(m_client_UserShown);
            m_client.JoinedChannel += new ServerChatEventHandler(m_client_JoinedChannel);
        }

        void m_client_JoinedChannel(object sender, ServerChatEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Invokee<ServerChatEventArgs>(ClearChannelList), e);
            else
                ClearChannelList(e);
        }

        private void ClearChannelList(ServerChatEventArgs e)
        {
            this.listBox1.Items.Clear();
            this.Text = this.TabText = "Channel: " + e.Text;
        }

        void m_client_UserShown(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
            else
                ShowUser(e);
        }

        private void ShowUser(UserEventArgs args)
        {
            if (!listBox1.Items.Contains(args))
            {
                if ((args.Flags & UserFlags.ChannelOperator) == UserFlags.None)
                {
                    this.listBox1.Items.Add(args);
                }
                else
                {
                    int count = this.listBox1.Items.OfType<UserEventArgs>().Count(a => (a.Flags & UserFlags.ChannelOperator) == UserFlags.ChannelOperator);
                    this.listBox1.Items.Insert(count, args);
                }
            }
        }

        void m_client_UserLeft(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(RemoveUser), e);
            else
                RemoveUser(e);
        }

        private void RemoveUser(UserEventArgs args)
        {
            this.listBox1.Items.Remove(args.Username);
        }

        void m_client_UserJoined(object sender, UserEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Invokee<UserEventArgs>(ShowUser), e);
            else
                ShowUser(e);
        }

        private delegate void Invokee<T>(T args);
    }
}
