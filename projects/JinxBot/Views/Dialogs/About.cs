using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace JinxBot.Views.Dialogs
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void wb_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Process.Start(e.Url.ToString());
            e.Cancel = true;
        }
    }
}
