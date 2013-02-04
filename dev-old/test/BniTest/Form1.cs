using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.MBNCSUtil.Data;
using System.Drawing.Drawing2D;

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
            try
            {
                BniFileParser bni = new BniFileParser(@"C:\Projects\jinxbot\branches\mbncsutil\trunk\mbnftp\bin\Debug\icons.bni");
                this.pictureBox1.Image = bni.FullImage;

                foreach (BniIcon icon in bni.AllIcons)
                {
                    this.flowContainer.Controls.Add(new IconRep(icon));
                }
            }
            catch { }

            Bitmap bmp = new Bitmap(28, 12);
            Rectangle bounds = new Rectangle(0, 0, 28, 12);
            using (Graphics g = Graphics.FromImage(bmp))
            using (LinearGradientBrush b = new LinearGradientBrush(bounds, Color.Black, Color.Black, 0, false))
            {
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { Color.LimeGreen, Color.Lime, Color.Yellow, Color.Orange, Color.OrangeRed, Color.Maroon };
                cb.Positions = new float[] { 0f, 0.1f, 0.4f, 0.6f, 0.9f, 1f };
                b.InterpolationColors = cb;

                g.FillRectangle(b, bounds);
            }
            pictureBox2.Image = bmp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
