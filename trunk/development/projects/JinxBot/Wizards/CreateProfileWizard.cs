using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Design;
using JinxBot.Configuration;
using System.Globalization;
using BNSharp.BattleNet;

namespace JinxBot.Wizards
{
    public partial class CreateProfileWizard : Form
    {
        private ClientProfile m_cp;
        public CreateProfileWizard()
        {
            InitializeComponent();

            m_cp = new ClientProfile();
            this.propertyGrid1.SelectedObject = new TypeDescriptorSurrogate(m_cp);
        }

        private void wc_FinishButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_cp.ProfileName))
            {
                m_cp.ProfileName = string.Format(CultureInfo.CurrentCulture, "{0} on {1} ({2})", m_cp.Username, m_cp.Server, Product.GetByProductCode(m_cp.Client).Name);
            }
            JinxBotConfiguration.Instance.AddProfile(m_cp);
            JinxBotConfiguration.Instance.Save();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
