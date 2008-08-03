using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using JinxBot.Controls;

namespace JinxBot.Views
{
    public partial class ErrorLogDocument : DockableDocument
    {
        public ErrorLogDocument()
        {
            InitializeComponent();
        }

        public void AddError(string message)
        {
            display.AddChat(new ChatNode(message, Color.Orange));
        }
    }
}
