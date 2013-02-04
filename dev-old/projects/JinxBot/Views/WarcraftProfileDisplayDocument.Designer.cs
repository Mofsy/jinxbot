namespace JinxBot.Views
{
    partial class WarcraftProfileDisplayDocument
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
            this.about = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.homepage = new System.Windows.Forms.LinkLabel();
            this.clanName = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ladderStats = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.arrTeamStats = new System.Windows.Forms.FlowLayoutPanel();
            this.clanStats = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.personalStats = new JinxBot.Views.Stats.RaceStatsDisplay();
            this.clanRecords = new JinxBot.Views.Stats.RaceStatsDisplay();
            this.clanRank = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(14, 117);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(294, 51);
            this.about.TabIndex = 24;
            this.about.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "About:";
            // 
            // homepage
            // 
            this.homepage.AutoSize = true;
            this.homepage.Location = new System.Drawing.Point(123, 80);
            this.homepage.Name = "homepage";
            this.homepage.Size = new System.Drawing.Size(59, 13);
            this.homepage.TabIndex = 22;
            this.homepage.TabStop = true;
            this.homepage.Text = "Homepage";
            this.homepage.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // clanName
            // 
            this.clanName.AutoSize = true;
            this.clanName.Location = new System.Drawing.Point(123, 39);
            this.clanName.Name = "clanName";
            this.clanName.Size = new System.Drawing.Size(88, 13);
            this.clanName.TabIndex = 21;
            this.clanName.Text = "Valhalla Legends";
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.Location = new System.Drawing.Point(118, 10);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(68, 25);
            this.name.TabIndex = 20;
            this.name.Text = "Name";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(11, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 84);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(315, 9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(260, 357);
            this.tabControl1.TabIndex = 26;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ladderStats);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(252, 331);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Ladder Stats";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.arrTeamStats);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(252, 331);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Arranged Team Stats";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ladderStats
            // 
            this.ladderStats.AutoScroll = true;
            this.ladderStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ladderStats.Location = new System.Drawing.Point(3, 3);
            this.ladderStats.Name = "ladderStats";
            this.ladderStats.Size = new System.Drawing.Size(246, 325);
            this.ladderStats.TabIndex = 26;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.clanStats);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(252, 331);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Clan Stats";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // arrTeamStats
            // 
            this.arrTeamStats.AutoScroll = true;
            this.arrTeamStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrTeamStats.Location = new System.Drawing.Point(3, 3);
            this.arrTeamStats.Name = "arrTeamStats";
            this.arrTeamStats.Size = new System.Drawing.Size(246, 325);
            this.arrTeamStats.TabIndex = 27;
            // 
            // clanStats
            // 
            this.clanStats.AutoScroll = true;
            this.clanStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clanStats.Location = new System.Drawing.Point(3, 3);
            this.clanStats.Name = "clanStats";
            this.clanStats.Size = new System.Drawing.Size(246, 325);
            this.clanStats.TabIndex = 27;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Location = new System.Drawing.Point(2, 141);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(303, 225);
            this.tabControl2.TabIndex = 27;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.personalStats);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(295, 199);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Personal Race Records";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.clanRecords);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(295, 199);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "Clan Race Records";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // personalStats
            // 
            this.personalStats.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.personalStats.Location = new System.Drawing.Point(-1, 5);
            this.personalStats.Name = "personalStats";
            this.personalStats.Size = new System.Drawing.Size(297, 188);
            this.personalStats.TabIndex = 19;
            // 
            // clanRecords
            // 
            this.clanRecords.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clanRecords.Location = new System.Drawing.Point(-1, 5);
            this.clanRecords.Name = "clanRecords";
            this.clanRecords.Size = new System.Drawing.Size(297, 188);
            this.clanRecords.TabIndex = 20;
            // 
            // clanRank
            // 
            this.clanRank.AutoSize = true;
            this.clanRank.Location = new System.Drawing.Point(123, 60);
            this.clanRank.Name = "clanRank";
            this.clanRank.Size = new System.Drawing.Size(88, 13);
            this.clanRank.TabIndex = 28;
            this.clanRank.Text = "Valhalla Legends";
            // 
            // WarcraftProfileDisplayDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 378);
            this.Controls.Add(this.clanRank);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.about);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.homepage);
            this.Controls.Add(this.clanName);
            this.Controls.Add(this.name);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarcraftProfileDisplayDocument";
            this.ShowIcon = false;
            this.TabText = "Profile Display :: (none)";
            this.Text = "Profile Display :: (none)";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label about;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel homepage;
        private System.Windows.Forms.Label clanName;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.FlowLayoutPanel ladderStats;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.FlowLayoutPanel arrTeamStats;
        private System.Windows.Forms.FlowLayoutPanel clanStats;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private JinxBot.Views.Stats.RaceStatsDisplay personalStats;
        private System.Windows.Forms.TabPage tabPage5;
        private JinxBot.Views.Stats.RaceStatsDisplay clanRecords;
        private System.Windows.Forms.Label clanRank;




    }
}