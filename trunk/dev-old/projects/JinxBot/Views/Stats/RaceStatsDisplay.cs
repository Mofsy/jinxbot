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
    internal partial class RaceStatsDisplay : UserControl
    {
        private class Row
        {
            public Label Win, Loss, Percent;
        }

        private Dictionary<Warcraft3IconRace, Row> rowMap;
        private Row totalsRow;

        public RaceStatsDisplay()
        {
            InitializeComponent();

            this.hw.Text = hl.Text = hp.Text = rw.Text = rl.Text = rp.Text = ow.Text = ol.Text = op.Text = uw.Text = ul.Text = up.Text =
                nw.Text = nl.Text = np.Text = tw.Text = tl.Text = tp.Text = wins.Text = losses.Text = pct.Text = "0";

            rowMap = new Dictionary<Warcraft3IconRace, Row> { 
                { Warcraft3IconRace.Random, new Row { Win = rw, Loss = rl, Percent = rp }  },
                { Warcraft3IconRace.Human, new Row { Win = hw, Loss = hl, Percent = hp } },
                { Warcraft3IconRace.Orc, new Row { Win = ow, Loss = ol, Percent = op } },
                { Warcraft3IconRace.Undead, new Row { Win = uw, Loss = ul, Percent = up } },
                { Warcraft3IconRace.NightElf, new Row { Win = nw, Loss = nl, Percent = np } },
                { Warcraft3IconRace.Tournament, new Row { Win = tw, Loss = tl, Percent = tp } }
            };

            totalsRow = new Row { Win = wins, Loss = losses, Percent = pct };
        }

        private bool m_exp = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specifies whether the viewed user is using the Warcraft III Expansion.")]
        [DefaultValue(true)]
        public bool IsExpansion
        {
            get
            {
                return m_exp;
            }
            set
            {
                m_exp = value;
                tt.Visible = tw.Visible = tl.Visible = tp.Visible = value;
            }
        }

        public void BindToStats(IEnumerable<WarcraftRaceRecord> raceRecords)
        {
            foreach (var record in raceRecords)
            {
                Row row = rowMap[record.Race];
                row.Win.Text = record.Wins.ToString(CultureInfo.CurrentCulture);
                row.Loss.Text = record.Losses.ToString(CultureInfo.CurrentCulture);
                double percent = record.Wins * 100.0 / (double)(record.Wins + record.Losses);
                row.Percent.Text = string.Format(CultureInfo.CurrentCulture, "{0:F}%", percent);
            }

            totalsRow.Win.Text = raceRecords.Sum(r => r.Wins).ToString(CultureInfo.CurrentCulture);
            totalsRow.Loss.Text = raceRecords.Sum(r => r.Losses).ToString(CultureInfo.CurrentCulture);
            totalsRow.Percent.Text = string.Format(CultureInfo.CurrentCulture, "{0:F}%",
                (double)raceRecords.Sum(r => r.Wins * 100.0) / (double)(raceRecords.Sum(r => r.Wins + r.Losses)));
        }
    }
}
