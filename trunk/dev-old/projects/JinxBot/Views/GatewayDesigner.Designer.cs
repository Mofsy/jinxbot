namespace JinxBot.Views
{
    partial class GatewayDesigner
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
            this.rbDefault = new System.Windows.Forms.RadioButton();
            this.cbDefaults = new System.Windows.Forms.ComboBox();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.customName = new System.Windows.Forms.TextBox();
            this.customOldSfx = new System.Windows.Forms.TextBox();
            this.customNewSfx = new System.Windows.Forms.TextBox();
            this.customServer = new System.Windows.Forms.TextBox();
            this.customPort = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.customPort)).BeginInit();
            this.SuspendLayout();
            // 
            // rbDefault
            // 
            this.rbDefault.AutoSize = true;
            this.rbDefault.Checked = true;
            this.rbDefault.Location = new System.Drawing.Point(12, 15);
            this.rbDefault.Name = "rbDefault";
            this.rbDefault.Size = new System.Drawing.Size(149, 17);
            this.rbDefault.TabIndex = 0;
            this.rbDefault.TabStop = true;
            this.rbDefault.Text = "Select a Default Gateway:";
            this.rbDefault.UseVisualStyleBackColor = true;
            // 
            // cbDefaults
            // 
            this.cbDefaults.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDefaults.FormattingEnabled = true;
            this.cbDefaults.Items.AddRange(new object[] {
            "US East - Azeroth",
            "US West - Lordaeron",
            "Europe - Northrend",
            "Asia - Kalimdor"});
            this.cbDefaults.Location = new System.Drawing.Point(39, 38);
            this.cbDefaults.Name = "cbDefaults";
            this.cbDefaults.Size = new System.Drawing.Size(233, 21);
            this.cbDefaults.TabIndex = 1;
            this.cbDefaults.SelectedIndexChanged += new System.EventHandler(this.cbDefaults_SelectedIndexChanged);
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(12, 81);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(142, 17);
            this.rbCustom.TabIndex = 2;
            this.rbCustom.Text = "Enter a custom gateway:";
            this.rbCustom.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(243, 83);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(29, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Old Client Suffix:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Warcraft 3 Client Suffix:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Server (Name or IP):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Port (Default is 6112):";
            // 
            // customName
            // 
            this.customName.Location = new System.Drawing.Point(128, 108);
            this.customName.Name = "customName";
            this.customName.Size = new System.Drawing.Size(144, 20);
            this.customName.TabIndex = 9;
            // 
            // customOldSfx
            // 
            this.customOldSfx.Location = new System.Drawing.Point(128, 134);
            this.customOldSfx.Name = "customOldSfx";
            this.customOldSfx.Size = new System.Drawing.Size(144, 20);
            this.customOldSfx.TabIndex = 10;
            // 
            // customNewSfx
            // 
            this.customNewSfx.Location = new System.Drawing.Point(128, 160);
            this.customNewSfx.Name = "customNewSfx";
            this.customNewSfx.Size = new System.Drawing.Size(144, 20);
            this.customNewSfx.TabIndex = 11;
            // 
            // customServer
            // 
            this.customServer.Location = new System.Drawing.Point(128, 186);
            this.customServer.Name = "customServer";
            this.customServer.Size = new System.Drawing.Size(144, 20);
            this.customServer.TabIndex = 12;
            // 
            // customPort
            // 
            this.customPort.Location = new System.Drawing.Point(152, 212);
            this.customPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.customPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.customPort.Name = "customPort";
            this.customPort.Size = new System.Drawing.Size(120, 20);
            this.customPort.TabIndex = 13;
            this.customPort.Value = new decimal(new int[] {
            6112,
            0,
            0,
            0});
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(197, 238);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(116, 238);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // GatewayDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 271);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.customPort);
            this.Controls.Add(this.customServer);
            this.Controls.Add(this.customNewSfx);
            this.Controls.Add(this.customOldSfx);
            this.Controls.Add(this.customName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.cbDefaults);
            this.Controls.Add(this.rbDefault);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GatewayDesigner";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gateway Selector";
            ((System.ComponentModel.ISupportInitialize)(this.customPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDefault;
        private System.Windows.Forms.ComboBox cbDefaults;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox customName;
        private System.Windows.Forms.TextBox customOldSfx;
        private System.Windows.Forms.TextBox customNewSfx;
        private System.Windows.Forms.TextBox customServer;
        private System.Windows.Forms.NumericUpDown customPort;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

    }
}