using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using BNSharp.BattleNet;
using JinxBot.Util;

namespace JinxBot.Views
{
    public partial class GatewayDesigner : Form
    {
        private static Gateway[] DefaultGateways = new Gateway[] 
        {
            Gateway.USEast, 
            Gateway.USWest,
            Gateway.Europe,
            Gateway.Asia
        };

        public GatewayDesigner()
        {
            InitializeComponent();
            RefreshEnabledStates();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.jinxbot.net/wiki/JinxBot_Gateway_Selector");
        }

        private void cbDefaults_SelectedIndexChanged(object sender, EventArgs e)
        {
            Gateway defaultGw = DefaultGateways[cbDefaults.SelectedIndex];

            customName.Text = defaultGw.Name;
            customOldSfx.Text = defaultGw.OldClientSuffix;
            customNewSfx.Text = defaultGw.Warcraft3ClientSuffix;
            customServer.Text = defaultGw.ServerHost;
            customPort.Value = defaultGw.ServerPort;
        }

        public Gateway ChosenGateway
        {
            get
            {
                if (rbDefault.Checked)
                {
                    return DefaultGateways[cbDefaults.SelectedIndex];
                }
                else
                {
                    return new Gateway(
                        customName.Text,
                        customOldSfx.Text,
                        customNewSfx.Text,
                        customServer.Text,
                        (int)customPort.Value
                        );
                }
            }
            set
            {
                int index = DefaultGateways.IndexOf(value);
                if (index >= 0)
                {
                    rbDefault.Checked = true;
                    cbDefaults.SelectedIndex = DefaultGateways.IndexOf(value);
                }
                else
                {
                    rbCustom.Checked = true;
                }

                RefreshEnabledStates();

                customName.Text = value.Name;
                customNewSfx.Text = value.Warcraft3ClientSuffix;
                customOldSfx.Text = value.OldClientSuffix;
                customServer.Text = value.ServerHost;
                customPort.Value = value.ServerPort;
            }
        }

        private void RefreshEnabledStates()
        {
            bool useDefault = rbDefault.Checked;
            bool useCustom = !useDefault;

            cbDefaults.Enabled = useDefault;
            customName.Enabled = customOldSfx.Enabled = 
                customNewSfx.Enabled = customServer.Enabled = 
                customPort.Enabled = useCustom;
        }
    }
}
