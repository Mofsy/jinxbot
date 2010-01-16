using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp;
using JinxBot.Configuration;
using System.IO;

namespace JinxBot.Views
{
    public partial class ProfileLoadErrorDialog : Form
    {
        public ProfileLoadErrorDialog()
        {
            InitializeComponent();

            this.label2.Text = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "Settings.xml");
        }

        public ProfileLoadErrorDialog(BattleNetSettingsErrorsException ex)
           : this()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < (int.MaxValue >> 2); i <<= 1)
            {
                BattleNetSettingsErrors errorBeingTested = (BattleNetSettingsErrors)i;
                if ((ex.Errors & errorBeingTested) == errorBeingTested)
                {
                    sb.AppendLine(FormatError(errorBeingTested));
                }
            }
            this.textBox1.Text = sb.ToString();
        }

        private string FormatError(BattleNetSettingsErrors errorBeingTested)
        {
            string item = Resources.ResourceManager.GetString("BattleNetSettingsErrors_" + errorBeingTested.ToString());
            if (string.IsNullOrEmpty(item))
                item = errorBeingTested.ToString();

            item = "\u2022 " + item + Environment.NewLine;

            return item;
        }
    }
}
