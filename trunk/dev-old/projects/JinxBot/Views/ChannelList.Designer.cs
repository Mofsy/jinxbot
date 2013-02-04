namespace JinxBot.Views
{
    partial class ChannelList
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
            this.components = new System.ComponentModel.Container();
            this.ttChannelTip = new System.Windows.Forms.ToolTip(this.components);
            this.listBox1 = new JinxBot.Views.Chat.CustomDrawnSearchableListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.squelchUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.inviteUserToClanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beginFormingANewClanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addUserAsAFriendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookUpUserProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ttChannelTip
            // 
            this.ttChannelTip.AutoPopDelay = 5000;
            this.ttChannelTip.InitialDelay = 500;
            this.ttChannelTip.OwnerDraw = true;
            this.ttChannelTip.ReshowDelay = 100;
            this.ttChannelTip.ShowAlways = true;
            this.ttChannelTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ttChannelTip_Popup);
            this.ttChannelTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ttChannelTip_Draw);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.ForeColor = System.Drawing.Color.Snow;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(282, 539);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseHover += new System.EventHandler(this.listBox1_MouseHover);
            this.listBox1.FilteringItem += new System.EventHandler<JinxBot.Views.Chat.ItemFilteringEventArgs>(this.listBox1_FilteringItem);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            this.listBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseMove);
            this.listBox1.MouseLeave += new System.EventHandler(this.listBox1_MouseLeave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.squelchUserToolStripMenuItem,
            this.kickUserToolStripMenuItem,
            this.banUserToolStripMenuItem,
            this.toolStripMenuItem1,
            this.inviteUserToClanToolStripMenuItem,
            this.beginFormingANewClanToolStripMenuItem,
            this.toolStripMenuItem2,
            this.addUserAsAFriendToolStripMenuItem,
            this.lookUpUserProfileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(216, 192);
            // 
            // squelchUserToolStripMenuItem
            // 
            this.squelchUserToolStripMenuItem.Name = "squelchUserToolStripMenuItem";
            this.squelchUserToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.squelchUserToolStripMenuItem.Text = "Squelch/Unsquelch User";
            // 
            // kickUserToolStripMenuItem
            // 
            this.kickUserToolStripMenuItem.Name = "kickUserToolStripMenuItem";
            this.kickUserToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.kickUserToolStripMenuItem.Text = "Kick User";
            // 
            // banUserToolStripMenuItem
            // 
            this.banUserToolStripMenuItem.Name = "banUserToolStripMenuItem";
            this.banUserToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.banUserToolStripMenuItem.Text = "Ban User";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(212, 6);
            // 
            // inviteUserToClanToolStripMenuItem
            // 
            this.inviteUserToClanToolStripMenuItem.Name = "inviteUserToClanToolStripMenuItem";
            this.inviteUserToClanToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.inviteUserToClanToolStripMenuItem.Text = "Invite User to Clan";
            // 
            // beginFormingANewClanToolStripMenuItem
            // 
            this.beginFormingANewClanToolStripMenuItem.Enabled = false;
            this.beginFormingANewClanToolStripMenuItem.Name = "beginFormingANewClanToolStripMenuItem";
            this.beginFormingANewClanToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.beginFormingANewClanToolStripMenuItem.Text = "Begin Forming a New Clan";
            this.beginFormingANewClanToolStripMenuItem.Click += new System.EventHandler(this.beginFormingANewClanToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(212, 6);
            // 
            // addUserAsAFriendToolStripMenuItem
            // 
            this.addUserAsAFriendToolStripMenuItem.Name = "addUserAsAFriendToolStripMenuItem";
            this.addUserAsAFriendToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.addUserAsAFriendToolStripMenuItem.Text = "Add User as a Friend";
            // 
            // lookUpUserProfileToolStripMenuItem
            // 
            this.lookUpUserProfileToolStripMenuItem.Name = "lookUpUserProfileToolStripMenuItem";
            this.lookUpUserProfileToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.lookUpUserProfileToolStripMenuItem.Text = "Look Up User Profile";
            // 
            // ChannelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 539);
            this.Controls.Add(this.listBox1);
            this.Name = "ChannelList";
            this.TabText = "Channel";
            this.Text = "Channel";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JinxBot.Views.Chat.CustomDrawnSearchableListBox listBox1;
        private System.Windows.Forms.ToolTip ttChannelTip;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem squelchUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem banUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem inviteUserToClanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beginFormingANewClanToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addUserAsAFriendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lookUpUserProfileToolStripMenuItem;


    }
}