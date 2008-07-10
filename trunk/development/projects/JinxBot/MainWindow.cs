using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using BNSharp.Net;
using JinxBot.Views;
using JinxBot.Wizards;
using JinxBot.Configuration;

namespace JinxBot
{
    public partial class MainWindow : Form
    {
        private BattleNetClient m_bnc;

        public MainWindow()
        {
            InitializeComponent();

            JinxBotConfiguration.Instance.ProfileAdded += new EventHandler(Instance_ProfileAdded);
            JinxBotConfiguration.Instance.ProfileRemoved += new EventHandler(Instance_ProfileRemoved);
        }

        void Instance_ProfileRemoved(object sender, EventArgs e)
        {
            RebindProfiles();
        }

        private void RebindProfiles()
        {
            this.profilesToolStripMenuItem.DropDownItems.Clear();
            foreach (ClientProfile p in JinxBotConfiguration.Instance.Profiles)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(p.ProfileName);
                tsmi.Tag = p;
                tsmi.Click += new EventHandler(tsmi_Click);
                this.profilesToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }

        void tsmi_Click(object sender, EventArgs e)
        {
            ClientProfile cp = (sender as ToolStripMenuItem).Tag as ClientProfile;
            if (cp != null)
            {
                BattleNetClient bnc = new BattleNetClient(cp);
                ProfileResourceProvider.RegisterProvider(bnc);
                ProfileDocument profile = new ProfileDocument(bnc);
                profile.Show(this.dock);
            }
        }

        void Instance_ProfileAdded(object sender, EventArgs e)
        {
            RebindProfiles();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProfileDisplay pd = new ProfileDisplay();
            pd.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileDocument pd = this.dock.ActiveDocument as ProfileDocument;
            if (pd != null)
            {
                BattleNetClient bnc = pd.Client;
                bnc.Connect();
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            RebindProfiles();

            if (!JinxBotConfiguration.ConfigurationFileExists)
            {
                FirstRunWizard wizard = new FirstRunWizard();
                wizard.ShowDialog(this);
            }

            if (JinxBotConfiguration.Instance.Profiles.Length == 0)
            {
                CreateProfileWizard cpw = new CreateProfileWizard();
                cpw.ShowDialog(this);
            }
        }

        private void newProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateProfileWizard cpw = new CreateProfileWizard();
            cpw.ShowDialog(this);
        }
    }
}
