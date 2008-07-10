using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.MBNCSUtil.Data;

namespace BniTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BniFileParser bni = new BniFileParser(@"C:\Projects\mbncsutil\trunk\mbnftp\bin\x64\Debug\icons.bni");
            this.pictureBox1.Image = bni.FullImage;

            foreach (BniIcon icon in bni.AllIcons)
            {
                this.flowContainer.Controls.Add(new IconRep(icon));
            }
        }
    }
}
