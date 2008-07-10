using System.Security.Permissions;
namespace JinxBot.Controls
{
    partial class DisplayBox
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
            this.display = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.AllowNavigation = false;
            this.display.AllowWebBrowserDrop = false;
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.IsWebBrowserContextMenuEnabled = false;
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.MinimumSize = new System.Drawing.Size(20, 20);
            this.display.Name = "display";
            this.display.ScriptErrorsSuppressed = true;
            this.display.Size = new System.Drawing.Size(464, 229);
            this.display.TabIndex = 0;
            this.display.WebBrowserShortcutsEnabled = false;
            this.display.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.display_DocumentCompleted);
            // 
            // DisplayBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.display);
            this.Name = "DisplayBox";
            this.Size = new System.Drawing.Size(464, 229);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser display;
    }
}
