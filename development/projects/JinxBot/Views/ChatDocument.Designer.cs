namespace JinxBot.Views
{
    partial class ChatDocument
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
            this.chat = new JinxBot.Controls.ChatBox();
            this.SuspendLayout();
            // 
            // chat
            // 
            this.chat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chat.Location = new System.Drawing.Point(0, 0);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(608, 393);
            this.chat.TabIndex = 0;
            this.chat.MessageReady += new JinxBot.Controls.MessageEventHandler(this.chat_MessageReady);
            // 
            // ChatDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 393);
            this.Controls.Add(this.chat);
            this.Name = "ChatDocument";
            this.TabText = "ChatDocument";
            this.Text = "ChatDocument";
            this.ResumeLayout(false);

        }

        #endregion

        private JinxBot.Controls.ChatBox chat;
    }
}