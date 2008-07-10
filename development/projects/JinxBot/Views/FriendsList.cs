using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using BNSharp.Net;

namespace JinxBot.Views
{
    public partial class FriendsList : DockableToolWindow
    {
        protected FriendsList()
        {
            InitializeComponent();
        }

        public FriendsList(BattleNetClient client)
            : this()
        {

        }
    }
}
