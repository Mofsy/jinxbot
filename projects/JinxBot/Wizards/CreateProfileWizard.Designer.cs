namespace JinxBot.Wizards
{
    partial class CreateProfileWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateProfileWizard));
            this.wc = new WizardBase.WizardControl();
            this.startStep1 = new WizardBase.StartStep();
            this.intermediateStep1 = new WizardBase.IntermediateStep();
            this.finishStep1 = new WizardBase.FinishStep();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.label9 = new System.Windows.Forms.Label();
            this.intermediateStep1.SuspendLayout();
            this.finishStep1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wc
            // 
            this.wc.BackButtonEnabled = true;
            this.wc.BackButtonVisible = false;
            this.wc.CancelButtonEnabled = true;
            this.wc.CancelButtonVisible = false;
            this.wc.EulaButtonEnabled = true;
            this.wc.EulaButtonText = "eula";
            this.wc.EulaButtonVisible = false;
            this.wc.HelpButtonEnabled = true;
            this.wc.HelpButtonVisible = false;
            this.wc.Location = new System.Drawing.Point(0, 0);
            this.wc.Name = "wc";
            this.wc.NextButtonEnabled = true;
            this.wc.NextButtonVisible = true;
            this.wc.Size = new System.Drawing.Size(534, 403);
            this.wc.WizardSteps.AddRange(new WizardBase.WizardStep[] {
            this.startStep1,
            this.intermediateStep1,
            this.finishStep1});
            this.wc.FinishButtonClick += new System.EventHandler(this.wc_FinishButtonClick);
            // 
            // startStep1
            // 
            this.startStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("startStep1.BindingImage")));
            this.startStep1.Icon = ((System.Drawing.Image)(resources.GetObject("startStep1.Icon")));
            this.startStep1.Name = "startStep1";
            this.startStep1.Subtitle = "This wizard will help you through the steps required to create a Battle.net clien" +
                "t profile for use in JinxBot.";
            this.startStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.SubtitleAppearence")));
            this.startStep1.Title = "Welcome to the Profile Creation Wizard.";
            this.startStep1.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.TitleAppearence")));
            // 
            // intermediateStep1
            // 
            this.intermediateStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("intermediateStep1.BindingImage")));
            this.intermediateStep1.Controls.Add(this.propertyGrid1);
            this.intermediateStep1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.intermediateStep1.Name = "intermediateStep1";
            this.intermediateStep1.Subtitle = "Please fill out the following settings.";
            this.intermediateStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("intermediateStep1.SubtitleAppearence")));
            this.intermediateStep1.Title = "Settings";
            // 
            // finishStep1
            // 
            this.finishStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("finishStep1.BindingImage")));
            this.finishStep1.Controls.Add(this.label9);
            this.finishStep1.Name = "finishStep1";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(12, 62);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(507, 298);
            this.propertyGrid1.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(49, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(172, 39);
            this.label9.TabIndex = 1;
            this.label9.Text = "Thank you for setting up JinxBot.\r\n\r\nClick Finish to complete the wizard.";
            // 
            // CreateProfileWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 404);
            this.ControlBox = false;
            this.Controls.Add(this.wc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateProfileWizard";
            this.Text = "JinxBot Profile Creation Wizard";
            this.intermediateStep1.ResumeLayout(false);
            this.finishStep1.ResumeLayout(false);
            this.finishStep1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WizardBase.WizardControl wc;
        private WizardBase.StartStep startStep1;
        private WizardBase.IntermediateStep intermediateStep1;
        private WizardBase.FinishStep finishStep1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label label9;
    }
}