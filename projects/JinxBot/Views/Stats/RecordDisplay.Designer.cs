namespace JinxBot.Views.Stats
{
    partial class RecordDisplay
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
            this.recordTitle = new System.Windows.Forms.Label();
            this.level = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.exp = new System.Windows.Forms.Label();
            this.wins = new System.Windows.Forms.Label();
            this.losses = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rank = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // recordTitle
            // 
            this.recordTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordTitle.Location = new System.Drawing.Point(3, 0);
            this.recordTitle.Name = "recordTitle";
            this.recordTitle.Size = new System.Drawing.Size(222, 22);
            this.recordTitle.TabIndex = 0;
            this.recordTitle.Text = "label1";
            this.recordTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // level
            // 
            this.level.Location = new System.Drawing.Point(0, 22);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(225, 17);
            this.level.TabIndex = 1;
            this.level.Text = "Level 99";
            this.level.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(30, 42);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(164, 19);
            this.progress.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Exp:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Wins:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Losses:";
            // 
            // exp
            // 
            this.exp.Location = new System.Drawing.Point(59, 68);
            this.exp.Name = "exp";
            this.exp.Size = new System.Drawing.Size(53, 13);
            this.exp.TabIndex = 6;
            this.exp.Text = "100,000";
            this.exp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // wins
            // 
            this.wins.Location = new System.Drawing.Point(59, 81);
            this.wins.Name = "wins";
            this.wins.Size = new System.Drawing.Size(53, 13);
            this.wins.TabIndex = 7;
            this.wins.Text = "100,000";
            this.wins.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // losses
            // 
            this.losses.Location = new System.Drawing.Point(59, 94);
            this.losses.Name = "losses";
            this.losses.Size = new System.Drawing.Size(53, 13);
            this.losses.TabIndex = 8;
            this.losses.Text = "100,000";
            this.losses.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Rank:";
            // 
            // rank
            // 
            this.rank.AutoSize = true;
            this.rank.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rank.Location = new System.Drawing.Point(155, 78);
            this.rank.Name = "rank";
            this.rank.Size = new System.Drawing.Size(67, 16);
            this.rank.TabIndex = 10;
            this.rank.Text = "Unranked";
            // 
            // RecordDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Name = "RecordDisplay";
            this.Size = new System.Drawing.Size(225, 126);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label recordTitle;
        private System.Windows.Forms.Label level;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label exp;
        private System.Windows.Forms.Label wins;
        private System.Windows.Forms.Label losses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label rank;
    }
}
