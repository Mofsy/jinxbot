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
using System.Reflection;
using JinxBot.Plugins;
using JinxBot.Reliability;

namespace JinxBot
{
    public partial class MainWindow : Form
    {
        private Dictionary<ClientProfile, ProfileDocument> m_activeProfiles;

        public MainWindow()
        {
            InitializeComponent();

            m_activeProfiles = new Dictionary<ClientProfile, ProfileDocument>();

            JinxBotConfiguration.Instance.ProfileAdded += new EventHandler(Instance_ProfileAdded);
            JinxBotConfiguration.Instance.ProfileRemoved += new EventHandler(Instance_ProfileRemoved);

            // show and hide again to ensure that web browser catches up
            GlobalErrorHandler.ErrorLog.Show(dock);
            GlobalErrorHandler.ErrorLog.Hide();
        }

        void Instance_ProfileRemoved(object sender, EventArgs e)
        {
            RebindProfiles();
        }

        private void RebindProfiles()
        {
            ClearProfilesList();
            foreach (ClientProfile p in JinxBotConfiguration.Instance.Profiles)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(p.ProfileName);
                tsmi.Tag = p;
                tsmi.Click += new EventHandler(tsmi_Click);
                this.profilesToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }

        private void ClearProfilesList()
        {
            while (profilesToolStripMenuItem.DropDownItems.Count > 2)
            {
                profilesToolStripMenuItem.DropDownItems.RemoveAt(2);
            }
        }

        void tsmi_Click(object sender, EventArgs e)
        {
            ClientProfile cp = (sender as ToolStripMenuItem).Tag as ClientProfile;
            if (cp != null)
            {
                if (m_activeProfiles.ContainsKey(cp))
                {
                    m_activeProfiles[cp].Show();
                }
                else
                {
                    BattleNetClient bnc = new BattleNetClient(cp);
                    ProfileResourceProvider.RegisterProvider(bnc);
                    ProfileDocument profile = new ProfileDocument(bnc);
                    m_activeProfiles.Add(cp, profile);
                    profile.Show(this.dock);

                    if (cp.ProfileName == "DT on East - D2DV")
                    {
                        Assembly asm = Assembly.LoadFile(Path.Combine(Application.StartupPath, "JinxBot.Plugins.JinxBotWeb.dll"));
                        Type t = asm.GetType("JinxBot.Plugins.JinxBotWeb.JinxBotWebPlugin", false, false);
                        IEventListener listener = Activator.CreateInstance(t) as IEventListener;
                        if (listener != null)
                        {
                            listener.HandleClientStartup(bnc);
                        }
                    }
                }
            }
        }

        void Instance_ProfileAdded(object sender, EventArgs e)
        {
            RebindProfiles();
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

        private void dock_ActiveDocumentChanged(object sender, EventArgs e)
        {
            ProfileDocument profileDoc = this.dock.ActiveDocument as ProfileDocument;
            if (profileDoc != null)
            {
                this.currentProfileNoneToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.currentProfileNoneToolStripMenuItem.Enabled = false;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileDocument pd = this.dock.ActiveDocument as ProfileDocument;
            if (pd != null)
            {
                if (pd.Client.IsConnected)
                    pd.Client.Close();

                ClientProfile profile = pd.Client.Settings as ClientProfile;
                pd.Close();

                m_activeProfiles.Remove(profile);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileDocument pd = this.dock.ActiveDocument as ProfileDocument;
            if (pd != null)
            {
                BattleNetClient bnc = pd.Client;
                bnc.Close();
            }
        }

        private void displayErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayErrorLogToolStripMenuItem.Checked)
            {
                GlobalErrorHandler.ErrorLog.Hide();
                displayErrorLogToolStripMenuItem.Checked = false;
            }
            else
            {
                GlobalErrorHandler.ErrorLog.Show(this.dock);
                displayErrorLogToolStripMenuItem.Checked = true;
            }
        }
    }
}
