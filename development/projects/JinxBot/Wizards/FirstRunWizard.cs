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

namespace JinxBot.Wizards
{
    public partial class FirstRunWizard : Form
    {
        private static string[] IconsList = new string[] { 
            "http://www.battle.net/war3/images/battle.net/icons/tier1-orc.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier2-human.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier3-human.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier4-human.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier5-human.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier2-orc.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier3-orc.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier4-orc.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier5-orc.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier2-nightelf.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier3-nightelf.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier4-nightelf.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier5-nightelf.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier2-undead.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier3-undead.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier4-undead.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier5-undead.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier2-random.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier3-random.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier4-random.gif",
            "http://www.battle.net/war3/images/battle.net/icons/tier5-random.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3H2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3H3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3H4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3H5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3H6.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3O2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3O3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3O4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3O5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3O6.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3N2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3N3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3N4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3N5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3N6.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3U2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3U3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3U4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3U5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3U6.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3R2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3R3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3R4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3R5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3R6.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3T2.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3T3.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3T4.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3T5.gif",
            "http://www.battle.net/war3/ladder/portraits/large/w3xp/W3T6.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-blizzard.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-battlenet.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-speaker.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-squelch.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-channelops.gif",
            "http://www.battle.net/war3/images/battle.net/war3x-icons/war3x.gif",
            "http://www.battle.net/war3/images/battle.net/war3x-icons/war3.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-diablo.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-diablo2.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-diablo2exp.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-starcraft.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-starcraftexp.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-war2bne.gif",
            "http://www.battle.net/war3/images/battle.net/icons/bnet-unknown.gif"
        };
            
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
                for (int i = 0; i < IconsList.Length; i++)
                {
                    bwDownload.ReportProgress(i * 100 / IconsList.Length, IconsList[i]);
                    DownloadFile(IconsList[i]);
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

        private void DownloadFile(string path)
        {
            string localPath = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "Icons");
            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            localPath = Path.Combine(localPath, path.Substring(path.LastIndexOf('/') + 1));
            WebClient client = new WebClient();
            client.DownloadFile(path, localPath);
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
