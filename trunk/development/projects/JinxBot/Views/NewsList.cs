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
using BNSharp.BattleNet;
using System.Threading;

namespace JinxBot.Views
{
    public partial class NewsList : DockableToolWindow
    {
        private BattleNetClient m_client;

        public NewsList()
        {
            InitializeComponent();

            this.DockAreas = DockAreas.DockLeft;
        }

        public NewsList(BattleNetClient client)
            : this()
        {
            m_client = client;

            m_client.ServerNews += new BNSharp.BattleNet.ServerNewsEventHandler(m_client_ServerNews);
        }

        void m_client_ServerNews(object sender, BNSharp.BattleNet.ServerNewsEventArgs e)
        {
            ThreadStart del = delegate
            {
                foreach (NewsEntry ne in e.Entries)
                {
                    comboBox1.Items.Add(ne);
                }

                if (e.Entries.Length > 0)
                {
                    this.comboBox1.SelectedIndex = 0;
                    comboBox1_SelectedIndexChanged(this, e);
                }
            };
            if (InvokeRequired)
                BeginInvoke(del);
            else
                del();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblNews.Text = (this.comboBox1.SelectedItem as NewsEntry).News;
        }
    }
}
