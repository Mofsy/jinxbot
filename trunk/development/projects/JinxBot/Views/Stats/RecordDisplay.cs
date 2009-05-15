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
    public partial class RecordDisplay : UserControl
    {
        public RecordDisplay()
        {
            InitializeComponent();
        }

        public RecordDisplay(WarcraftLadderRecord record)
            : this()
        {
            switch (record.LadderType)
            {
                case WarcraftLadderType.Solo:
                    recordTitle.Text = "Solo Games";
                    break;
                case WarcraftLadderType.Team:
                    recordTitle.Text = "Team Games";
                    break;
                case WarcraftLadderType.FreeForAll:
                    recordTitle.Text = "FFA Games";
                    break;
            }

            level.Text = string.Format(CultureInfo.CurrentUICulture, "Level {0}", record.Level);
            exp.Text = record.TotalExperience.ToString("#,##0", CultureInfo.CurrentUICulture);
            wins.Text = record.Wins.ToString("#,##0", CultureInfo.CurrentUICulture);
            losses.Text = record.Losses.ToString("#,##0", CultureInfo.CurrentUICulture);
            rank.Text = (record.Rank == 0) ? "Unranked" : record.Rank.ToString(CultureInfo.CurrentUICulture);
            progress.Value = Math.Min(record.HoursUntilExperienceDecay, 100);
        }

        public RecordDisplay(WarcraftClanLadderRecord record)
            : this()
        {
            switch (record.LadderType)
            {
                case WarcraftClanLadderType.Solo:
                    recordTitle.Text = "Solo Games";
                    break;
                case WarcraftClanLadderType.TwoVsTwo:
                    recordTitle.Text = "2 vs 2 Games";
                    break;
                case WarcraftClanLadderType.ThreeVsThree:
                    recordTitle.Text = "3 vs 3 Games";
                    break;
                case WarcraftClanLadderType.FourVsFour:
                    recordTitle.Text = "4 vs 4 Games";
                    break;
            }

            level.Text = string.Format(CultureInfo.CurrentUICulture, "Level {0}", record.Level);
            exp.Text = record.TotalExperience.ToString("#,##0", CultureInfo.CurrentUICulture);
            wins.Text = record.Wins.ToString("#,##0", CultureInfo.CurrentUICulture);
            losses.Text = record.Losses.ToString("#,##0", CultureInfo.CurrentUICulture);
            rank.Text = (record.Rank == 0) ? "Unranked" : record.Rank.ToString(CultureInfo.CurrentUICulture);
            progress.Value = Math.Min(record.HoursUntilExperienceDecay, 100);
        }
    }
}
