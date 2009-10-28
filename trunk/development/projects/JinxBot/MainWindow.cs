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
using JinxBot.Plugins.UI;
using BNSharp.BattleNet;
using JinxBot.Plugins.BNSharp;
using JinxBot.Controls.Docking;
using Microsoft.WindowsAPICodePack.Taskbar;
using JinxBot.Windows7;
using System.Threading;

namespace JinxBot
{
    public partial class MainWindow : Form, IMainWindow, IJumpListWindowTarget
    {
        private Dictionary<ClientProfile, JinxBotClient> m_activeClients;

        private string[] m_programArgs;

        public MainWindow()
        {
            InitializeComponent();

            m_activeClients = new Dictionary<ClientProfile, JinxBotClient>();

            JinxBotConfiguration.Instance.ProfileAdded += new EventHandler(Instance_ProfileAdded);
            JinxBotConfiguration.Instance.ProfileRemoved += new EventHandler(Instance_ProfileRemoved);

            // show and hide again to ensure that web browser catches up
            GlobalErrorHandler.ErrorLog.Show(dock);
            //GlobalErrorHandler.ErrorLog.Hide();

            //this.menuStrip1.Renderer = new JinxBotMenuRenderer();

            PluginFactory.MainWindow = this;
            
            InstanceManagementService.HostIMS(this);
        }

        public MainWindow(string[] args)
            : this()
        {
            m_programArgs = args;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

            JumpListManager.RefreshJumpList(JinxBotConfiguration.Instance.Profiles);
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
            LoadOrDisplayProfile(cp);
        }

        private void LoadOrDisplayProfile(ClientProfile cp)
        {
            if (cp != null)
            {
                if (m_activeClients.ContainsKey(cp))
                {
                    m_activeClients[cp].ProfileDocument.Show();
                }
                else
                {
                    JinxBotClient client = new JinxBotClient(cp);
                    client.Client.Connected += client_Connected;
                    client.Client.Disconnected += client_Disconnected;
                    m_activeClients.Add(cp, client);
                    client.ProfileDocument.Show(this.dock);

                    ThumbnailPreviewManager.AddThumbnail(client.ProfileDocument);
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
                pd.BeginConnecting();

                this.connectToolStripMenuItem1.Enabled = false;
                this.disconnectToolStripMenuItem.Enabled = true;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

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

            ThumbnailPreviewManager.Initialize(this);

            if (m_programArgs != null && m_programArgs.Length > 0)
            {
                (this as IJumpListWindowTarget).HandleJumpListCall(m_programArgs);
                m_programArgs = null;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private delegate void SyncDel();

        private void client_Connected(object sender, EventArgs e)
        {
            BattleNetClient client = sender as BattleNetClient;
            if (client == null) return;
            ProfileDocument profileDoc = this.dock.ActiveDocument as ProfileDocument;
            if (profileDoc == null) return;
            if (profileDoc.Client == client)
            {
                SyncDel go = delegate
                {
                    this.connectToolStripMenuItem1.Enabled = false;
                    this.disconnectToolStripMenuItem.Enabled = true;
                };
                if (InvokeRequired)
                    BeginInvoke(go);
                else
                    go();
            }
        }

        private void client_Disconnected(object sender, EventArgs e)
        {
            BattleNetClient client = sender as BattleNetClient;
            if (client == null) return;
            ProfileDocument profileDoc = this.dock.ActiveDocument as ProfileDocument;
            if (profileDoc == null) return;
            if (profileDoc.Client == client)
            {
                SyncDel go = delegate
                {
                    this.connectToolStripMenuItem1.Enabled = true;
                    this.disconnectToolStripMenuItem.Enabled = false;
                };
                if (InvokeRequired)
                    BeginInvoke(go);
                else
                    go();
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
                this.enableVoidViewToolStripMenuItem.Checked = profileDoc.VoidView;
                this.connectToolStripMenuItem1.Enabled = !profileDoc.Client.IsConnected;
                this.disconnectToolStripMenuItem.Enabled = !connectToolStripMenuItem1.Enabled;
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
                ClientProfile profile = pd.Client.Settings as ClientProfile;

                JinxBotClient client = m_activeClients[profile];
                client.Client.Disconnected -= client_Disconnected;
                client.Client.Connected -= client_Connected;
                client.Close();

                pd.Close();

                m_activeClients.Remove(profile);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileDocument pd = this.dock.ActiveDocument as ProfileDocument;
            if (pd != null)
            {
                pd.Disconnect();

                this.disconnectToolStripMenuItem.Enabled = false;
                this.connectToolStripMenuItem1.Enabled = true;
            }
        }

        private void displayErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayErrorLogToolStripMenuItem.Checked)
            {
                GlobalErrorHandler.ErrorLog.Hide();
                displayErrorLogToolStripMenuItem.Checked = false;

                ThumbnailPreviewManager.RemoveThumbnail(GlobalErrorHandler.ErrorLog);
            }
            else
            {
                GlobalErrorHandler.ErrorLog.Show(this.dock);
                displayErrorLogToolStripMenuItem.Checked = true;
                ThumbnailPreviewManager.AddThumbnail(GlobalErrorHandler.ErrorLog);
            }
        }

        private void defaultStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IChatTab tab = this.dock.ActiveDocument as IChatTab;
            if (tab != null)
            {
                string jbControlsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JinxBot.Controls");
                string defaultPath = Path.Combine(jbControlsPath, "DefaultStyles.css");
                Uri styleUri = new Uri(string.Concat("file:///", defaultPath));
                tab.StylesheetUri = styleUri;
            }
        }

        private void blizzardStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IChatTab tab = this.dock.ActiveDocument as IChatTab;
            if (tab != null)
            {
                string jbControlsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JinxBot.Controls");
                string defaultPath = Path.Combine(jbControlsPath, "BlizzStyles.css");
                Uri styleUri = new Uri(string.Concat("file:///", defaultPath));
                tab.StylesheetUri = styleUri;
            }
        }

        private void enableVoidViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileDocument tab = this.dock.ActiveDocument as ProfileDocument;
            if (tab != null)
            {
                tab.VoidView = enableVoidViewToolStripMenuItem.Checked;
            }
        }

        #region IMainWindow Members

        public void AddDocument(DockableDocument mainWindowDocument)
        {
            mainWindowDocument.Show(this.dock);
        }

        public void RemoveDocument(DockableDocument mainWindowDocument)
        {
            mainWindowDocument.Hide();
        }

        #endregion

        #region IJumpListWindowTarget Members

        void IJumpListWindowTarget.HandleJumpListCall(string[] param)
        {
            string p = param[0];
            if (p.StartsWith("--load-profile-"))
            {
                int profileIndex = int.Parse(p.Substring(p.LastIndexOf('-') + 1));
                ClientProfile profile = JinxBotConfiguration.Instance.Profiles[profileIndex];

                var ts = (ThreadStart)(() => LoadOrDisplayProfile(profile));
                if (InvokeRequired)
                    BeginInvoke(ts);
                else
                    ts();
            }
            else if (p.Equals("--new-profile"))
            {
                var ts = (ThreadStart)(() => newProfileToolStripMenuItem_Click(this, EventArgs.Empty));
                if (InvokeRequired)
                    BeginInvoke(ts);
                else
                    ts();
            }
        }

        #endregion
    }
}
