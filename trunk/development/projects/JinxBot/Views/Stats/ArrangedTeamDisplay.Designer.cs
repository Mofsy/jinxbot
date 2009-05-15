namespace JinxBot.Views.Stats
{
    partial class ArrangedTeamDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.losses = new System.Windows.Forms.Label();
            this.wins = new System.Windows.Forms.Label();
            this.exp = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.level = new System.Windows.Forms.Label();
            this.recordTitle = new System.Windows.Forms.Label();
            this.rank = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.partners = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Rank:";
            // 
            // losses
            // 
            this.losses.Location = new System.Drawing.Point(59, 96);
            this.losses.Name = "losses";
            this.losses.Size = new System.Drawing.Size(53, 13);
            this.losses.TabIndex = 19;
            this.losses.Text = "100,000";
            this.losses.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // wins
            // 
            this.wins.Location = new System.Drawing.Point(59, 83);
            this.wins.Name = "wins";
            this.wins.Size = new System.Drawing.Size(53, 13);
            this.wins.TabIndex = 18;
            this.wins.Text = "100,000";
            this.wins.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // exp
            // 
            this.exp.Location = new System.Drawing.Point(59, 70);
            this.exp.Name = "exp";
            this.exp.Size = new System.Drawing.Size(53, 13);
            this.exp.TabIndex = 17;
            this.exp.Text = "100,000";
            this.exp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Losses:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Wins:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Exp:";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(30, 44);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(164, 19);
            this.progress.TabIndex = 13;
            // 
            // level
            // 
            this.level.Location = new System.Drawing.Point(0, 24);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(225, 17);
            this.level.TabIndex = 12;
            this.level.Text = "Level 99";
            this.level.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // recordTitle
            // 
            this.recordTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordTitle.Location = new System.Drawing.Point(3, 2);
            this.recordTitle.Name = "recordTitle";
            this.recordTitle.Size = new System.Drawing.Size(222, 22);
            this.recordTitle.TabIndex = 11;
            this.recordTitle.Text = "label1";
            this.recordTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // rank
            // 
            this.rank.Location = new System.Drawing.Point(49, 109);
            this.rank.Name = "rank";
            this.rank.Size = new System.Drawing.Size(63, 13);
            this.rank.TabIndex = 21;
            this.rank.Text = "Unranked";
            this.rank.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(118, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Partners:";
            // 
            // partners
            // 
            this.partners.Location = new System.Drawing.Point(121, 83);
            this.partners.Name = "partners";
            this.partners.Size = new System.Drawing.Size(101, 39);
            this.partners.TabIndex = 23;
            this.partners.Text = "Lt.Deadlock-AoA, DarkTemplar-AoA, sno-man";
            // 
            // ArrangedTeamDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.partners);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rank);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.losses);
            this.Controls.Add(this.wins);
            this.Controls.Add(this.exp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.level);
            this.Controls.Add(this.recordTitle);
            this.Name = "ArrangedTeamDisplay";
            this.Size = new System.Drawing.Size(225, 126);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label losses;
        private System.Windows.Forms.Label wins;
        private System.Windows.Forms.Label exp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label level;
        private System.Windows.Forms.Label recordTitle;
        private System.Windows.Forms.Label rank;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label partners;
    }
}
