using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.ChatLog
{
    public partial class ChatReplayControls : DockableToolWindow
    {
        public ChatReplayControls()
        {
            InitializeComponent();
            this.DockAreas = DockAreas.DockBottom | DockAreas.Float;
        }
    }
}
