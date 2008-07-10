namespace JinxBot.Controls.Design
{
    partial class FlagsEditorUI<T>
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
            this.clbFlags = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // clbFlags
            // 
            this.clbFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbFlags.CheckOnClick = true;
            this.clbFlags.FormattingEnabled = true;
            this.clbFlags.Location = new System.Drawing.Point(3, 3);
            this.clbFlags.Name = "clbFlags";
            this.clbFlags.Size = new System.Drawing.Size(169, 169);
            this.clbFlags.TabIndex = 0;
            this.clbFlags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbFlags_ItemCheck);
            // 
            // FlagsEditorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clbFlags);
            this.Name = "FlagsEditorUI";
            this.Size = new System.Drawing.Size(174, 175);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbFlags;
    }
}
