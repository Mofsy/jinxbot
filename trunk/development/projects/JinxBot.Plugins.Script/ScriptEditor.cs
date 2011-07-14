using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.Script
{
    public partial class ScriptEditor : DockableDocument
    {
        public ScriptEditor()
        {
            InitializeComponent();
        }

        public string ScriptCode
        {
            get { return tbScript.Text; }
            set { tbScript.Text = value; }
        }

        private void tbScript_TextChanged(object sender, EventArgs e)
        {
            OnScriptChanged(e);
        }

        protected virtual void OnScriptChanged(EventArgs e)
        {
            if (ScriptChanged != null)
                ScriptChanged(this, e);
        }

        public event EventHandler ScriptChanged;
    }
}
