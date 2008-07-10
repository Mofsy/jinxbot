using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Design;

namespace JinxBot.Views
{
    public partial class ProfileSettingsEditor : UserControl
    {
        private ClientProfile m_profile;

        public ProfileSettingsEditor()
        {
            InitializeComponent();
        }

        public ProfileSettingsEditor(ClientProfile profile)
            : this()
        {
            m_profile = profile;
            TypeDescriptorSurrogate tds = new TypeDescriptorSurrogate(profile);
            this.pg.SelectedObject = tds;
        }

        public ClientProfile Profile
        {
            get { return m_profile; }
            set
            {
                m_profile = value;
                if (!object.ReferenceEquals(value, null))
                {
                    TypeDescriptorSurrogate tds = new TypeDescriptorSurrogate(value);
                    this.pg.SelectedObject = tds;
                }
            }
        }
    }
}
