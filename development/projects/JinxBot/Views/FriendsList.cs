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
using BNSharp.BattleNet.Friends;
using System.Threading;
using BNSharp.BattleNet;

namespace JinxBot.Views
{
    public partial class FriendsList : DockableToolWindow
    {
        protected FriendsList()
        {
            InitializeComponent();
        }

        public FriendsList(BattleNetClient client)
            : this()
        {
            client.FriendListReceived += new FriendListReceivedEventHandler(client_FriendListReceived);
            client.FriendMoved += new FriendMovedEventHandler(client_FriendMoved);
            client.FriendAdded += new FriendAddedEventHandler(client_FriendAdded);
            client.FriendRemoved += new FriendRemovedEventHandler(client_FriendRemoved);
            client.FriendUpdated += new FriendUpdatedEventHandler(client_FriendUpdated);
        }

        void client_FriendUpdated(object sender, FriendUpdatedEventArgs e)
        {
            
        }

        void client_FriendRemoved(object sender, FriendRemovedEventArgs e)
        {
            ThreadStart ts = delegate
            {
                this.lbFriends.Items.Remove(e.Friend);
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }

        void client_FriendAdded(object sender, FriendAddedEventArgs e)
        {
            ThreadStart ts = delegate
            {
                this.lbFriends.Items.Add(e.NewFriend);
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }

        void client_FriendMoved(object sender, FriendMovedEventArgs e)
        {
            ThreadStart ts = delegate
            {
                this.lbFriends.Items.Remove(e.Friend);
                this.lbFriends.Items.Insert(e.NewIndex, e.Friend);
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }

        void client_FriendListReceived(object sender, FriendListReceivedEventArgs e)
        {
            ThreadStart ts = delegate
            {
                foreach (FriendUser friend in e.Friends)
                {
                    this.lbFriends.Items.Add(friend);
                }
            };
            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }
    }
}
