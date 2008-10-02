using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BNSharp.BattleNet.Stats
{
    [DataContract]
    public class WarcraftRaceRecord
    {
        [DataMember(Name = "Wins")]
        private int m_wins;
        [DataMember(Name = "Losses")]
        private int m_losses;
        [DataMember(Name = "Race")]
        private Warcraft3IconRace m_race;

        public WarcraftRaceRecord(Warcraft3IconRace race, int wins, int losses)
        {
            if (wins < 0)
                throw new ArgumentOutOfRangeException("wins");
            if (losses < 0)
                throw new ArgumentOutOfRangeException("losses");
            if (!Enum.IsDefined(typeof(Warcraft3IconRace), race))
                throw new InvalidEnumArgumentException("race", (int)race, typeof(Warcraft3IconRace));

            m_race = race;
            m_wins = wins;
            m_losses = losses;
        }

        /// <summary>
        /// Gets the number of wins for this race record.
        /// </summary>
        public int Wins
        {
            get { return m_wins; }
        }

        /// <summary>
        /// Gets the number of losses for this race record.
        /// </summary>
        public int Losses
        {
            get { return m_losses; }
        }

        /// <summary>
        /// Gets the race of this particular record.
        /// </summary>
        public Warcraft3IconRace Race
        {
            get { return m_race; }
        }
    }
}
