using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using BNSharp.MBNCSUtil.Data;

namespace Blp2Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageParser parser = ImageParser.Create(openFileDialog1.FileName);
                Image img = parser.GetMipmapImage(0);
                pictureBox1.Image = img;
            }
        }
    }
}
