namespace JinxBot.Views
{
    partial class ChannelList
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
            this.components = new System.ComponentModel.Container();
            this.ttChannelTip = new System.Windows.Forms.ToolTip(this.components);
            this.listBox1 = new JinxBot.Views.Chat.CustomDrawnSearchableListBox();
            this.SuspendLayout();
            // 
            // ttChannelTip
            // 
            this.ttChannelTip.AutoPopDelay = 5000;
            this.ttChannelTip.InitialDelay = 500;
            this.ttChannelTip.OwnerDraw = true;
            this.ttChannelTip.ReshowDelay = 100;
            this.ttChannelTip.ShowAlways = true;
            this.ttChannelTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ttChannelTip_Popup);
            this.ttChannelTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ttChannelTip_Draw);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.ForeColor = System.Drawing.Color.Snow;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(282, 539);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseHover += new System.EventHandler(this.listBox1_MouseHover);
            this.listBox1.FilteringItem += new System.EventHandler<JinxBot.Views.Chat.ItemFilteringEventArgs>(this.listBox1_FilteringItem);
            this.listBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseMove);
            this.listBox1.MouseLeave += new System.EventHandler(this.listBox1_MouseLeave);
            // 
            // ChannelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 539);
            this.Controls.Add(this.listBox1);
            this.Name = "ChannelList";
            this.TabText = "Channel";
            this.Text = "Channel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JinxBot.Views.Chat.CustomDrawnSearchableListBox listBox1;
        private System.Windows.Forms.ToolTip ttChannelTip;


    }
}