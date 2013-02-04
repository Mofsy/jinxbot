using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using BNSharp.BattleNet.Stats;
using JinxBot.Views.Stats;
using System.Globalization;
using BNSharp.BattleNet;

namespace JinxBot.Views
{
    public partial class WarcraftProfileDisplayDocument : DockableDocument
    {
        public WarcraftProfileDisplayDocument()
        {
            InitializeComponent();
        }

        public WarcraftProfileDisplayDocument(WarcraftProfile profile)
            : this()
        {
            foreach (var at in profile.ArrangedTeams)
            {
                ArrangedTeamDisplay atd = new ArrangedTeamDisplay(at);
                this.arrTeamStats.Controls.Add(atd);
            }

            foreach (var rec in profile.LadderRecords)
            {
                RecordDisplay rd = new RecordDisplay(rec);
                this.ladderStats.Controls.Add(rd);
            }

            this.clanName.Text = profile.ClanTag;
            this.personalStats.BindToStats(profile.RaceRecords);
            this.about.Text = profile.Description;
        }

        public WarcraftProfileDisplayDocument(WarcraftProfileEventArgs args)
            : this(args.Profile)
        {
            this.name.Text = args.Username;
            this.homepage.Text = args.Profile.Location;

            if (args.Clan != null)
            {
                foreach (var rec in args.Clan.LadderRecords)
                {
                    this.clanStats.Controls.Add(new RecordDisplay(rec));
                }

                this.clanName.Text = string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", args.Clan.ClanName, args.Profile.ClanTag);
                this.clanRecords.BindToStats(args.Clan.RaceRecords);
                this.clanRank.Text = args.Clan.Rank.ToString();
                this.personalStats.IsExpansion = (args.Product == Product.Warcraft3Expansion);
                this.clanRecords.IsExpansion = personalStats.IsExpansion;
            }
            else
            {
                clanName.Text = "";
            }
        }
    }
}
