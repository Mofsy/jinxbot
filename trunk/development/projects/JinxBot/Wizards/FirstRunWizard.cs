using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.MBNCSUtil.Net;
using BNSharp.BattleNet;
using System.IO;
using BNSharp.MBNCSUtil.Data;
using JinxBot.Configuration;
using System.Net;
using WizardBase;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using System.Diagnostics;
using Icon = JinxBot.Configuration.WebIconList.Icon;
using System.Runtime.Serialization;

namespace JinxBot.Wizards
{
    public partial class FirstRunWizard : Form
    {            
        public FirstRunWizard()
        {
            InitializeComponent();
        }

        private void FirstRunWizard_Load(object sender, EventArgs e)
        {
            BnFtpVersion1Request req = new BnFtpVersion1Request(Product.StarcraftRetail.ProductCode, "icons.bni", null);
            string path = Path.GetTempFileName();
            req.LocalFileName = path;
            try
            {
                req.ExecuteRequest();

                BniFileParser bni = new BniFileParser(path);
                this.pbIconsBni.Image = bni.AllIcons[18].Image;
            }
            catch { }

        }

        private void llWhyDownloadIcons_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.jinxbot.net/wiki/index.php?title=Downloading_user_list_images");
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.rbUseBnetWebsiteIcons.Checked = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.rbUseIconsBni.Checked = true;
        }

        private void rbUseIconsBni_CheckedChanged(object sender, EventArgs e)
        {
            this.wizardControl1.NextButtonEnabled = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            rbAllowUsage.Checked = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            rbDisallowUsage.Checked = true;
        }

        private void wizardControl1_NextButtonClick(object sender, GenericCancelEventArgs<WizardControl> e)
        {
            
        }

        private void bwDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            if (rbUseBnetWebsiteIcons.Checked)
            {
                WebIconList iconsList = null;

                string xml = Resources.WebIconsList;
                using (StringReader sr = new StringReader(xml))
                using (XmlTextReader xtr = new XmlTextReader(sr))
                {
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(WebIconList));
                        iconsList = ser.Deserialize(xtr) as WebIconList;
                    }
                    catch (Exception)
                    {
                        // TODO: Log the exception.
                        MessageBox.Show("There was an error loading the icons list.", "Error Downloading Icons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        iconsList = new WebIconList() { Icons = new WebIconList.Icon[0] };
                    }
                }

                for (int i = 0; i < iconsList.Icons.Length; i++)
                {
                    bwDownload.ReportProgress(i * 100 / iconsList.Icons.Length, iconsList.Icons[i].Uri);
                    DownloadFile(iconsList.Icons[i]);
                }
            }
            else
            {
                bwDownload.ReportProgress(0, "icons.bni");

                BnFtpVersion1Request req = new BnFtpVersion1Request("STAR", "icons.bni", null);
                req.LocalFileName = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "icons.bni");
                req.FilePartDownloaded += new DownloadStatusEventHandler(req_FilePartDownloaded);
                req.ExecuteRequest();
            }
        }

        private void DownloadFile(Icon icon)
        {
            string localPath = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "Icons");
            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            string temporaryPath = Path.GetTempFileName();
            localPath = Path.Combine(localPath, icon.LocalName);
            WebClient client = new WebClient();
            client.DownloadFile(icon.Uri, temporaryPath);

            using (Image source = Image.FromFile(temporaryPath))
            using (Image target = new Bitmap(64, 44, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(target))
            using (Brush back = new SolidBrush(Color.Black))
            {
                g.FillRectangle(back, new Rectangle(Point.Empty, target.Size));

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                if (icon.Crop)
                {
                    Rectangle sourceRect = new Rectangle(0, icon.Top, 64, 64 - icon.Bottom - icon.Top);
                    Rectangle destinationRectangle = new Rectangle(0, (44 - sourceRect.Height) / 2, 64, sourceRect.Height);
                    g.DrawImage(source, destinationRectangle, sourceRect, GraphicsUnit.Pixel);
                }
                else
                {
                    int requestedWidth = 64 * source.Height / 44; // should be 55; if not, we'll need to do complex scaling
                    Rectangle sourceRect = new Rectangle(Point.Empty, source.Size);
                    Rectangle destRect;
                    if (requestedWidth == source.Width)
                    {
                        // simple scaling
                        destRect = new Rectangle(Point.Empty, target.Size);
                    }
                    else
                    {
                        // first try to scale at 64 pixels wide.
                        int resultHeight = 64 * source.Height / source.Width;
                        if (resultHeight <= 44)
                        {
                            destRect = new Rectangle(0, (44 - resultHeight) / 2, 64, resultHeight);
                        }
                        else
                        {
                            // image is taller than destination proportions allow; 
                            // scale by 44 tall then
                            int resultWidth = source.Width * 44 / source.Height;
                            destRect = new Rectangle(0, 0, resultWidth, 44);
                        }
                    }

                    g.DrawImage(source, destRect, sourceRect, GraphicsUnit.Pixel);
                }

                target.Save(localPath, ImageFormat.Png);
            }

            File.Delete(temporaryPath);
        }

        void req_FilePartDownloaded(object sender, DownloadStatusEventArgs e)
        {
            bwDownload.ReportProgress(e.DownloadStatus * 100 / e.FileLength);
        }

        private const int STEP_DOWNLOADING = 2;
        private void wizardControl1_CurrentStepIndexChanged(object sender, EventArgs e)
        {
            if (wizardControl1.CurrentStepIndex == STEP_DOWNLOADING)
            {
                bwDownload.RunWorkerAsync();
            }
        }

        private void bwDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.pb.Value = e.ProgressPercentage;
            if (e.UserState != null)
                this.lblFileName.Text = e.UserState as string;
        }

        private void bwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.wizardControl1.CurrentStepIndex++;
        }

        private void wizardControl1_FinishButtonClick(object sender, EventArgs e)
        {
            JinxBotConfiguration.Instance.Globals.AllowDataCollection = this.rbAllowUsage.Checked;
            JinxBotConfiguration.Instance.Globals.IconType = this.rbUseBnetWebsiteIcons.Checked ? IconType.BattleNetWebSite : IconType.IconsBni;
            JinxBotConfiguration.Instance.Save();

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
