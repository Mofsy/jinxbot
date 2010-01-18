using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using BNSharp;
using System.Diagnostics;
using System.Threading;

namespace JinxBot.Views
{
    internal partial class ChannelSelect : DockableToolWindow
    {
        private ChannelListEventArgs m_channels;
        private JinxBotClient m_client;

        private ChannelSelect()
        {
            InitializeComponent();
        }

        public ChannelSelect(JinxBotClient client)
            : this()
        {
            Debug.Assert(client != null);

            m_client = client;
            m_client.Client.RegisterChannelListReceivedNotification(Priority.Low, ChannelListReceived);
            m_client.Client.RegisterDisconnectedNotification(Priority.Low, Disconnected);
        }

        protected override void OnClosed(EventArgs e)
        {
            m_client.Client.UnregisterDisconnectedNotification(Priority.Low, Disconnected);
            m_client.Client.UnregisterChannelListReceivedNotification(Priority.Low, ChannelListReceived);
            base.OnClosed(e);
        }

        private void Disconnected(object sender, EventArgs e)
        {
            availableChannels.Invoke((ThreadStart)delegate
            {
                availableChannels.Items.Clear();
            });
        }

        private void ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            availableChannels.Invoke((ThreadStart)delegate
            {
                m_channels = e;

                foreach (string channel in e.Channels)
                {
                    availableChannels.Items.Add(channel);
                }
            });
        }

        private void availableChannels_DoubleClick(object sender, EventArgs e)
        {
            string channel = availableChannels.SelectedItem as string;
            if (channel != null)
            {
                m_client.Client.JoinChannel(channel, BNSharp.BattleNet.JoinMethod.Default);
            }
        }
    }
}
