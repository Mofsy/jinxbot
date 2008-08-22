namespace JinxBot
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.currentProfileNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.editProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurePluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticallyStartThisProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticallyReconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableVoidViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blizzardStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gettingStartedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dock = new JinxBot.Controls.Docking.DockPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 450);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(775, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.profilesToolStripMenuItem,
            this.currentProfileNoneToolStripMenuItem,
            this.styleToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(775, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findPluginsToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.displayErrorLogToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // findPluginsToolStripMenuItem
            // 
            this.findPluginsToolStripMenuItem.Enabled = false;
            this.findPluginsToolStripMenuItem.Name = "findPluginsToolStripMenuItem";
            this.findPluginsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.findPluginsToolStripMenuItem.Text = "Find &Plugins...";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Enabled = false;
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for &Updates...";
            // 
            // displayErrorLogToolStripMenuItem
            // 
            this.displayErrorLogToolStripMenuItem.Name = "displayErrorLogToolStripMenuItem";
            this.displayErrorLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.displayErrorLogToolStripMenuItem.Text = "Display Error &Log";
            this.displayErrorLogToolStripMenuItem.Click += new System.EventHandler(this.displayErrorLogToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // profilesToolStripMenuItem
            // 
            this.profilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProfileToolStripMenuItem,
            this.toolStripMenuItem5});
            this.profilesToolStripMenuItem.Name = "profilesToolStripMenuItem";
            this.profilesToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.profilesToolStripMenuItem.Text = "&Profiles";
            // 
            // newProfileToolStripMenuItem
            // 
            this.newProfileToolStripMenuItem.Name = "newProfileToolStripMenuItem";
            this.newProfileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newProfileToolStripMenuItem.Text = "&New Profile";
            this.newProfileToolStripMenuItem.Click += new System.EventHandler(this.newProfileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
            // 
            // currentProfileNoneToolStripMenuItem
            // 
            this.currentProfileNoneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem1,
            this.disconnectToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.editProfileToolStripMenuItem,
            this.configurePluginsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.automaticallyStartThisProfileToolStripMenuItem,
            this.automaticallyReconnectToolStripMenuItem,
            this.enableVoidViewToolStripMenuItem});
            this.currentProfileNoneToolStripMenuItem.Enabled = false;
            this.currentProfileNoneToolStripMenuItem.Name = "currentProfileNoneToolStripMenuItem";
            this.currentProfileNoneToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.currentProfileNoneToolStripMenuItem.Text = "&Current Profile";
            // 
            // connectToolStripMenuItem1
            // 
            this.connectToolStripMenuItem1.Name = "connectToolStripMenuItem1";
            this.connectToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.connectToolStripMenuItem1.Size = new System.Drawing.Size(234, 22);
            this.connectToolStripMenuItem1.Text = "&Connect";
            this.connectToolStripMenuItem1.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(231, 6);
            // 
            // editProfileToolStripMenuItem
            // 
            this.editProfileToolStripMenuItem.Enabled = false;
            this.editProfileToolStripMenuItem.Name = "editProfileToolStripMenuItem";
            this.editProfileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.editProfileToolStripMenuItem.Text = "&Edit Profile...";
            // 
            // configurePluginsToolStripMenuItem
            // 
            this.configurePluginsToolStripMenuItem.Enabled = false;
            this.configurePluginsToolStripMenuItem.Name = "configurePluginsToolStripMenuItem";
            this.configurePluginsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.configurePluginsToolStripMenuItem.Text = "Configure &Plugins...";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(231, 6);
            // 
            // automaticallyStartThisProfileToolStripMenuItem
            // 
            this.automaticallyStartThisProfileToolStripMenuItem.CheckOnClick = true;
            this.automaticallyStartThisProfileToolStripMenuItem.Enabled = false;
            this.automaticallyStartThisProfileToolStripMenuItem.Name = "automaticallyStartThisProfileToolStripMenuItem";
            this.automaticallyStartThisProfileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.automaticallyStartThisProfileToolStripMenuItem.Text = "&Automatically Start this Profile";
            // 
            // automaticallyReconnectToolStripMenuItem
            // 
            this.automaticallyReconnectToolStripMenuItem.Checked = true;
            this.automaticallyReconnectToolStripMenuItem.CheckOnClick = true;
            this.automaticallyReconnectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.automaticallyReconnectToolStripMenuItem.Enabled = false;
            this.automaticallyReconnectToolStripMenuItem.Name = "automaticallyReconnectToolStripMenuItem";
            this.automaticallyReconnectToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.automaticallyReconnectToolStripMenuItem.Text = "Automatically &Reconnect";
            // 
            // enableVoidViewToolStripMenuItem
            // 
            this.enableVoidViewToolStripMenuItem.Checked = true;
            this.enableVoidViewToolStripMenuItem.CheckOnClick = true;
            this.enableVoidViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableVoidViewToolStripMenuItem.Name = "enableVoidViewToolStripMenuItem";
            this.enableVoidViewToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.enableVoidViewToolStripMenuItem.Text = "Enable &Void View";
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultStyleToolStripMenuItem,
            this.blizzardStyleToolStripMenuItem});
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.styleToolStripMenuItem.Text = "&Style";
            // 
            // defaultStyleToolStripMenuItem
            // 
            this.defaultStyleToolStripMenuItem.Name = "defaultStyleToolStripMenuItem";
            this.defaultStyleToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.defaultStyleToolStripMenuItem.Text = "&Default Style";
            this.defaultStyleToolStripMenuItem.Click += new System.EventHandler(this.defaultStyleToolStripMenuItem_Click);
            // 
            // blizzardStyleToolStripMenuItem
            // 
            this.blizzardStyleToolStripMenuItem.Name = "blizzardStyleToolStripMenuItem";
            this.blizzardStyleToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.blizzardStyleToolStripMenuItem.Text = "&Blizzard Style";
            this.blizzardStyleToolStripMenuItem.Click += new System.EventHandler(this.blizzardStyleToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gettingStartedToolStripMenuItem,
            this.toolStripMenuItem4,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // gettingStartedToolStripMenuItem
            // 
            this.gettingStartedToolStripMenuItem.Enabled = false;
            this.gettingStartedToolStripMenuItem.Name = "gettingStartedToolStripMenuItem";
            this.gettingStartedToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.gettingStartedToolStripMenuItem.Text = "&Getting Started";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(150, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // dock
            // 
            this.dock.ActiveAutoHideContent = null;
            this.dock.AllowEndUserNestedDocking = false;
            this.dock.DefaultFloatWindowSize = new System.Drawing.Size(640, 400);
            this.dock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dock.Location = new System.Drawing.Point(0, 24);
            this.dock.Name = "dock";
            this.dock.Size = new System.Drawing.Size(775, 426);
            this.dock.TabIndex = 2;
            this.dock.ActiveDocumentChanged += new System.EventHandler(this.dock_ActiveDocumentChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 472);
            this.Controls.Add(this.dock);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "JinxBot [alpha] :: boticulation evolved :: http://www.jinxbot.net/";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private JinxBot.Controls.Docking.DockPanel dock;
        private System.Windows.Forms.ToolStripMenuItem profilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentProfileNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem editProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurePluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gettingStartedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem automaticallyStartThisProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automaticallyReconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableVoidViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayErrorLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blizzardStyleToolStripMenuItem;

    }
}