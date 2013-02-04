namespace JinxBot.Views
{
    partial class ClanList
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
            this.lbClanMembers = new JinxBot.Views.Chat.CustomDrawnListBox();
            this.clanMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeRankMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chieftanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shamanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gruntToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromClanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setClanMessageOfTheDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clanMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbClanMembers
            // 
            this.lbClanMembers.BackColor = System.Drawing.Color.Black;
            this.lbClanMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbClanMembers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbClanMembers.ForeColor = System.Drawing.Color.White;
            this.lbClanMembers.FormattingEnabled = true;
            this.lbClanMembers.Location = new System.Drawing.Point(0, 0);
            this.lbClanMembers.Name = "lbClanMembers";
            this.lbClanMembers.Size = new System.Drawing.Size(274, 472);
            this.lbClanMembers.TabIndex = 0;
            // 
            // clanMenuStrip
            // 
            this.clanMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeRankMenuItem,
            this.removeFromClanToolStripMenuItem,
            this.setClanMessageOfTheDayToolStripMenuItem});
            this.clanMenuStrip.Name = "clanMenuStrip";
            this.clanMenuStrip.Size = new System.Drawing.Size(224, 92);
            // 
            // changeRankMenuItem
            // 
            this.changeRankMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chieftanToolStripMenuItem,
            this.shamanToolStripMenuItem,
            this.gruntToolStripMenuItem,
            this.peonToolStripMenuItem});
            this.changeRankMenuItem.Name = "changeRankMenuItem";
            this.changeRankMenuItem.Size = new System.Drawing.Size(223, 22);
            this.changeRankMenuItem.Text = "Change Rank";
            // 
            // chieftanToolStripMenuItem
            // 
            this.chieftanToolStripMenuItem.Name = "chieftanToolStripMenuItem";
            this.chieftanToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.chieftanToolStripMenuItem.Text = "Chieftan";
            // 
            // shamanToolStripMenuItem
            // 
            this.shamanToolStripMenuItem.Name = "shamanToolStripMenuItem";
            this.shamanToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.shamanToolStripMenuItem.Text = "Shaman";
            // 
            // gruntToolStripMenuItem
            // 
            this.gruntToolStripMenuItem.Name = "gruntToolStripMenuItem";
            this.gruntToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.gruntToolStripMenuItem.Text = "Grunt";
            // 
            // peonToolStripMenuItem
            // 
            this.peonToolStripMenuItem.Name = "peonToolStripMenuItem";
            this.peonToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.peonToolStripMenuItem.Text = "Peon";
            // 
            // removeFromClanToolStripMenuItem
            // 
            this.removeFromClanToolStripMenuItem.Name = "removeFromClanToolStripMenuItem";
            this.removeFromClanToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.removeFromClanToolStripMenuItem.Text = "Remove from Clan";
            // 
            // setClanMessageOfTheDayToolStripMenuItem
            // 
            this.setClanMessageOfTheDayToolStripMenuItem.Name = "setClanMessageOfTheDayToolStripMenuItem";
            this.setClanMessageOfTheDayToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.setClanMessageOfTheDayToolStripMenuItem.Text = "Set Clan Message of the Day";
            // 
            // ClanList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 472);
            this.Controls.Add(this.lbClanMembers);
            this.Name = "ClanList";
            this.TabText = "Clan List";
            this.Text = "Clan List";
            this.clanMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private JinxBot.Views.Chat.CustomDrawnListBox lbClanMembers;
        private System.Windows.Forms.ContextMenuStrip clanMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem changeRankMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chieftanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shamanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gruntToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem peonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFromClanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setClanMessageOfTheDayToolStripMenuItem;

    }
}