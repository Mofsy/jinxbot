using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.MBNCSUtil.Data;

namespace BniTest
{
    public partial class IconRep : UserControl
    {
        public IconRep()
        {
            InitializeComponent();
        }

        public IconRep(BniIcon icon)
            : this()
        {
            this.pictureBox1.Image = icon.Image;
            this.lbl.Text = icon.ToString();
        }
    }
}
