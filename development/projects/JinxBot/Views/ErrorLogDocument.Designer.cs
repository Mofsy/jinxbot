namespace JinxBot.Views
{
    partial class ErrorLogDocument
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
            this.display = new JinxBot.Controls.DisplayBox();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.MaxDisplayedParagraphs = 1000;
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(700, 394);
            this.display.TabIndex = 0;
            // 
            // ErrorLogDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 394);
            this.Controls.Add(this.display);
            this.Name = "ErrorLogDocument";
            this.TabText = "JinxBot Error Log";
            this.Text = "JinxBot Error Log";
            this.ResumeLayout(false);

        }

        #endregion

        private JinxBot.Controls.DisplayBox display;
    }
}