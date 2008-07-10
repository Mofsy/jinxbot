namespace JinxBot.Wizards
{
    partial class FirstRunWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstRunWizard));
            this.wizardControl1 = new WizardBase.WizardControl();
            this.startStep1 = new WizardBase.StartStep();
            this.stepDefineIcons = new WizardBase.IntermediateStep();
            this.pbIconsBni = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rbUseBnetWebsiteIcons = new System.Windows.Forms.RadioButton();
            this.llWhyDownloadIcons = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbUseIconsBni = new System.Windows.Forms.RadioButton();
            this.stepDownloading = new WizardBase.IntermediateStep();
            this.lblFileName = new System.Windows.Forms.Label();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.stepAllowData = new WizardBase.IntermediateStep();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rbAllowUsage = new System.Windows.Forms.RadioButton();
            this.rbDisallowUsage = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.finishStep1 = new WizardBase.FinishStep();
            this.label9 = new System.Windows.Forms.Label();
            this.bwDownload = new System.ComponentModel.BackgroundWorker();
            this.stepDefineIcons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconsBni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.stepDownloading.SuspendLayout();
            this.stepAllowData.SuspendLayout();
            this.finishStep1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.BackButtonEnabled = true;
            this.wizardControl1.BackButtonVisible = false;
            this.wizardControl1.CancelButtonEnabled = false;
            this.wizardControl1.CancelButtonVisible = true;
            this.wizardControl1.EulaButtonEnabled = false;
            this.wizardControl1.EulaButtonText = "eula";
            this.wizardControl1.EulaButtonVisible = false;
            this.wizardControl1.HelpButtonEnabled = false;
            this.wizardControl1.HelpButtonVisible = false;
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.NextButtonEnabled = true;
            this.wizardControl1.NextButtonVisible = true;
            this.wizardControl1.Size = new System.Drawing.Size(557, 434);
            this.wizardControl1.WizardSteps.AddRange(new WizardBase.WizardStep[] {
            this.startStep1,
            this.stepDefineIcons,
            this.stepDownloading,
            this.stepAllowData,
            this.finishStep1});
            this.wizardControl1.CurrentStepIndexChanged += new System.EventHandler(this.wizardControl1_CurrentStepIndexChanged);
            this.wizardControl1.FinishButtonClick += new System.EventHandler(this.wizardControl1_FinishButtonClick);
            this.wizardControl1.NextButtonClick += new WizardBase.GenericCancelEventHandler<WizardBase.WizardControl>(this.wizardControl1_NextButtonClick);
            // 
            // startStep1
            // 
            this.startStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("startStep1.BindingImage")));
            this.startStep1.Icon = ((System.Drawing.Image)(resources.GetObject("startStep1.Icon")));
            this.startStep1.Name = "startStep1";
            this.startStep1.Subtitle = "This wizard will help you configure JinxBot\'s initial settings.  These settings w" +
                "ill be used to support your global settings, such as where you\'ll get your image" +
                " data from.\r\n\r\nTo begin, click Next.";
            this.startStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.SubtitleAppearence")));
            this.startStep1.Title = "Welcome to the JinxBot First Run Wizard.";
            this.startStep1.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.TitleAppearence")));
            // 
            // stepDefineIcons
            // 
            this.stepDefineIcons.BindingImage = ((System.Drawing.Image)(resources.GetObject("stepDefineIcons.BindingImage")));
            this.stepDefineIcons.Controls.Add(this.pbIconsBni);
            this.stepDefineIcons.Controls.Add(this.pictureBox1);
            this.stepDefineIcons.Controls.Add(this.label3);
            this.stepDefineIcons.Controls.Add(this.label4);
            this.stepDefineIcons.Controls.Add(this.rbUseBnetWebsiteIcons);
            this.stepDefineIcons.Controls.Add(this.llWhyDownloadIcons);
            this.stepDefineIcons.Controls.Add(this.label2);
            this.stepDefineIcons.Controls.Add(this.label1);
            this.stepDefineIcons.Controls.Add(this.rbUseIconsBni);
            this.stepDefineIcons.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stepDefineIcons.Name = "stepDefineIcons";
            this.stepDefineIcons.Subtitle = "This step allows you to define how JinxBot gets its images for the user lists.";
            this.stepDefineIcons.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("stepDefineIcons.SubtitleAppearence")));
            this.stepDefineIcons.Title = "Configure View Images";
            this.stepDefineIcons.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("stepDefineIcons.TitleAppearence")));
            // 
            // pbIconsBni
            // 
            this.pbIconsBni.Location = new System.Drawing.Point(25, 139);
            this.pbIconsBni.Name = "pbIconsBni";
            this.pbIconsBni.Size = new System.Drawing.Size(56, 28);
            this.pbIconsBni.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIconsBni.TabIndex = 8;
            this.pbIconsBni.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ImageLocation = "http://www.battle.net/war3/images/battle.net/icons/tier1-orc.gif";
            this.pictureBox1.Location = new System.Drawing.Point(25, 235);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 38);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(145, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(311, 43);
            this.label3.TabIndex = 6;
            this.label3.Text = "This option will download a rich set of icons from the Battle.net web site.  Thes" +
                "e look like the WarCraft III Battle.net interface.  They take approximately 1mb " +
                "of disk space.";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(143, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(330, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Download the product icons from the Battle.net web site.";
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // rbUseBnetWebsiteIcons
            // 
            this.rbUseBnetWebsiteIcons.AutoSize = true;
            this.rbUseBnetWebsiteIcons.Location = new System.Drawing.Point(118, 246);
            this.rbUseBnetWebsiteIcons.Name = "rbUseBnetWebsiteIcons";
            this.rbUseBnetWebsiteIcons.Size = new System.Drawing.Size(14, 13);
            this.rbUseBnetWebsiteIcons.TabIndex = 4;
            this.rbUseBnetWebsiteIcons.TabStop = true;
            this.rbUseBnetWebsiteIcons.UseVisualStyleBackColor = true;
            this.rbUseBnetWebsiteIcons.CheckedChanged += new System.EventHandler(this.rbUseIconsBni_CheckedChanged);
            // 
            // llWhyDownloadIcons
            // 
            this.llWhyDownloadIcons.AutoSize = true;
            this.llWhyDownloadIcons.Location = new System.Drawing.Point(413, 371);
            this.llWhyDownloadIcons.Name = "llWhyDownloadIcons";
            this.llWhyDownloadIcons.Size = new System.Drawing.Size(129, 13);
            this.llWhyDownloadIcons.TabIndex = 3;
            this.llWhyDownloadIcons.TabStop = true;
            this.llWhyDownloadIcons.Text = "Why do I need to do this?";
            this.llWhyDownloadIcons.VisitedLinkColor = System.Drawing.Color.Blue;
            this.llWhyDownloadIcons.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llWhyDownloadIcons_LinkClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(147, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(311, 53);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(145, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Use a Starcraft icons.bni file.";
            this.label1.Click += new System.EventHandler(this.label2_Click);
            // 
            // rbUseIconsBni
            // 
            this.rbUseIconsBni.AutoSize = true;
            this.rbUseIconsBni.Location = new System.Drawing.Point(120, 147);
            this.rbUseIconsBni.Name = "rbUseIconsBni";
            this.rbUseIconsBni.Size = new System.Drawing.Size(14, 13);
            this.rbUseIconsBni.TabIndex = 0;
            this.rbUseIconsBni.TabStop = true;
            this.rbUseIconsBni.UseVisualStyleBackColor = true;
            this.rbUseIconsBni.CheckedChanged += new System.EventHandler(this.rbUseIconsBni_CheckedChanged);
            // 
            // stepDownloading
            // 
            this.stepDownloading.BindingImage = ((System.Drawing.Image)(resources.GetObject("stepDownloading.BindingImage")));
            this.stepDownloading.Controls.Add(this.lblFileName);
            this.stepDownloading.Controls.Add(this.pb);
            this.stepDownloading.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stepDownloading.Name = "stepDownloading";
            this.stepDownloading.Subtitle = "Please wait while the icons are downloaded.";
            this.stepDownloading.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("stepDownloading.SubtitleAppearence")));
            this.stepDownloading.Title = "Now Downloading Images";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(56, 151);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(0, 13);
            this.lblFileName.TabIndex = 1;
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(56, 121);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(445, 23);
            this.pb.TabIndex = 0;
            // 
            // stepAllowData
            // 
            this.stepAllowData.BindingImage = ((System.Drawing.Image)(resources.GetObject("stepAllowData.BindingImage")));
            this.stepAllowData.Controls.Add(this.label5);
            this.stepAllowData.Controls.Add(this.label7);
            this.stepAllowData.Controls.Add(this.label6);
            this.stepAllowData.Controls.Add(this.rbAllowUsage);
            this.stepAllowData.Controls.Add(this.rbDisallowUsage);
            this.stepAllowData.Controls.Add(this.label8);
            this.stepAllowData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stepAllowData.Name = "stepAllowData";
            this.stepAllowData.Subtitle = "Choose whether to allow JinxBot to periodically collect and report usage informat" +
                "ion.";
            this.stepAllowData.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("stepAllowData.SubtitleAppearence")));
            this.stepAllowData.Title = "Enable User Feedback?";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(101, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(381, 73);
            this.label5.TabIndex = 12;
            this.label5.Text = resources.GetString("label5.Text");
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(103, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(379, 108);
            this.label7.TabIndex = 9;
            this.label7.Text = resources.GetString("label7.Text");
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(99, 266);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(313, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Do not allow JinxBot to collect and report usage data.";
            this.label6.Click += new System.EventHandler(this.label5_Click);
            // 
            // rbAllowUsage
            // 
            this.rbAllowUsage.AutoSize = true;
            this.rbAllowUsage.Location = new System.Drawing.Point(76, 134);
            this.rbAllowUsage.Name = "rbAllowUsage";
            this.rbAllowUsage.Size = new System.Drawing.Size(14, 13);
            this.rbAllowUsage.TabIndex = 7;
            this.rbAllowUsage.TabStop = true;
            this.rbAllowUsage.UseVisualStyleBackColor = true;
            // 
            // rbDisallowUsage
            // 
            this.rbDisallowUsage.AutoSize = true;
            this.rbDisallowUsage.Checked = true;
            this.rbDisallowUsage.Location = new System.Drawing.Point(74, 287);
            this.rbDisallowUsage.Name = "rbDisallowUsage";
            this.rbDisallowUsage.Size = new System.Drawing.Size(14, 13);
            this.rbDisallowUsage.TabIndex = 10;
            this.rbDisallowUsage.TabStop = true;
            this.rbDisallowUsage.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(101, 113);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(272, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Allow JinxBot to collect and report usage data.";
            this.label8.Click += new System.EventHandler(this.label7_Click);
            // 
            // finishStep1
            // 
            this.finishStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("finishStep1.BindingImage")));
            this.finishStep1.Controls.Add(this.label9);
            this.finishStep1.Name = "finishStep1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(48, 116);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(172, 39);
            this.label9.TabIndex = 0;
            this.label9.Text = "Thank you for setting up JinxBot.\r\n\r\nClick Finish to complete the wizard.";
            // 
            // bwDownload
            // 
            this.bwDownload.WorkerReportsProgress = true;
            this.bwDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDownload_DoWork);
            this.bwDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDownload_RunWorkerCompleted);
            this.bwDownload.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDownload_ProgressChanged);
            // 
            // FirstRunWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 434);
            this.Controls.Add(this.wizardControl1);
            this.Name = "FirstRunWizard";
            this.Text = "JinxBot First Run Wizard";
            this.Load += new System.EventHandler(this.FirstRunWizard_Load);
            this.stepDefineIcons.ResumeLayout(false);
            this.stepDefineIcons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconsBni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.stepDownloading.ResumeLayout(false);
            this.stepDownloading.PerformLayout();
            this.stepAllowData.ResumeLayout(false);
            this.stepAllowData.PerformLayout();
            this.finishStep1.ResumeLayout(false);
            this.finishStep1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WizardBase.WizardControl wizardControl1;
        private WizardBase.StartStep startStep1;
        private WizardBase.IntermediateStep stepDefineIcons;
        private WizardBase.FinishStep finishStep1;
        private WizardBase.IntermediateStep stepDownloading;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbUseIconsBni;
        private System.Windows.Forms.LinkLabel llWhyDownloadIcons;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbUseBnetWebsiteIcons;
        private System.Windows.Forms.PictureBox pbIconsBni;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.ProgressBar pb;
        private System.ComponentModel.BackgroundWorker bwDownload;
        private WizardBase.IntermediateStep stepAllowData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbAllowUsage;
        private System.Windows.Forms.RadioButton rbDisallowUsage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}