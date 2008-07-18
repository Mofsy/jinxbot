using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MBNCSUtil.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Blp1Test
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
                MBNCSUtil.Data.BlpFileParser parser = new MBNCSUtil.Data.BlpFileParser(openFileDialog1.FileName);
                Image img = parser.GetMipmapImage(0);
                pictureBox1.Image = img;
                this.label2.Text = parser.CompressionType.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (MpqArchive archive = MpqServices.OpenArchive(openFileDialog1.FileName))
                    {
                        string fileList = archive.GetListFile();
                        StringReader sr = new StringReader(fileList);
                        while (sr.Peek() != -1)
                        {
                            string file = sr.ReadLine();
                            if (file.ToLower().EndsWith(".blp"))
                            {
                                archive.SaveToPath(file, folderBrowserDialog1.SelectedPath, true);
                            }
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
