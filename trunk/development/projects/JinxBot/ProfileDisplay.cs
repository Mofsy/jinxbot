using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace JinxBot
{
    public partial class ProfileDisplay : Form
    {
        public ProfileDisplay()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ClientProfile));
            using (FileStream fs = new FileStream(@"C:\Projects\Profile.xml", FileMode.OpenOrCreate, FileAccess.Write))
            {
                xs.Serialize(fs, this.profileSettingsEditor1.Profile);
            }
        }

        private void ProfileDisplay_Load(object sender, EventArgs e)
        {
            profileSettingsEditor1.Profile = new ClientProfile();
        }
    }
}
