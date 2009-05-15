using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BNSharp.BattleNet.Stats;
using System.Globalization;

namespace JinxBot.Views.Stats
{
    public partial class ArrangedTeamDisplay : UserControl
    {
        public ArrangedTeamDisplay()
        {
            InitializeComponent();
        }

        public ArrangedTeamDisplay(ArrangedTeamRecord record)
            : this()
        {
            switch (record.TeamType)
            {
                case ArrangedTeamType.TwoVsTwo:
                    recordTitle.Text = "2 vs 2";
                    break;
                case ArrangedTeamType.ThreeVsThree:
                    recordTitle.Text = "3 vs 3";
                    break;
                case ArrangedTeamType.FourVsFour:
                    recordTitle.Text = "4 vs 4";
                    break;
            }

            level.Text = string.Format(CultureInfo.CurrentUICulture, "Level {0}", record.Level);
            exp.Text = record.TotalExperience.ToString("#,##0", CultureInfo.CurrentUICulture);
            wins.Text = record.Wins.ToString("#,##0", CultureInfo.CurrentUICulture);
            losses.Text = record.Losses.ToString("#,##0", CultureInfo.CurrentUICulture);
            rank.Text = (record.Rank == 0) ? "Unranked" : record.Rank.ToString(CultureInfo.CurrentUICulture);
            progress.Value = Math.Min(record.HoursUntilExperienceDecay, 100);

            StringBuilder sb = new StringBuilder();
            foreach (string name in record.Teammates)
            {
                sb.AppendLine(name);
            }
            partners.Text = sb.ToString();
        }
    }
}
