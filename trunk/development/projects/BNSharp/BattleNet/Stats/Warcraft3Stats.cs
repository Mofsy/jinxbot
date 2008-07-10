using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.MBNCSUtil;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// Contains information about a user who is logged on with a Warcraft III client.
    /// </summary>
    public class Warcraft3Stats : UserStats
    {
        #region fields
        private static readonly Dictionary<char, Warcraft3IconRace> RaceMap = new Dictionary<char, Warcraft3IconRace> 
        { 
            { 'R', Warcraft3IconRace.Random },
            { 'T', Warcraft3IconRace.Tournament },
            { 'H', Warcraft3IconRace.Human },
            { 'O', Warcraft3IconRace.Orc },
            { 'U', Warcraft3IconRace.Undead },
            { 'N', Warcraft3IconRace.NightElf }
        };

        private Product m_prod;
        private Warcraft3IconRace m_race;
        private int m_iconTier, m_level;
        private string m_clanTag;
        #endregion

        #region constructor
        internal Warcraft3Stats(byte[] stats)
        {
            DataReader dr = new DataReader(stats);
            string productCode = dr.ReadDwordString(0);
            m_prod = Product.GetByProductCode(productCode);
            if (m_prod == null)
                m_prod = Product.UnknownProduct;
            
            dr.Seek(1);
            string iconInfo = dr.ReadDwordString((byte)' ');
            char raceID = iconInfo[2];
            if (RaceMap.ContainsKey(raceID))
            {
                m_race = RaceMap[raceID];
            }
            else
                m_race = Warcraft3IconRace.Unknown;

            m_iconTier = (int)(iconInfo[3] - '0');

            dr.Seek(1);
            string sLevel = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sLevel, out m_level);
            if (m_level == 0)
                m_level = 1;

            try
            {
                m_clanTag = dr.ReadDwordString((byte)' ');
            }
            catch { }

        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the user's clan tag.
        /// </summary>
        public string ClanTag
        {
            get { return m_clanTag; }
        }

        /// <summary>
        /// Gets the icon race used for the user.
        /// </summary>
        public Warcraft3IconRace IconRace
        {
            get { return m_race; }
        }

        /// <summary>
        /// Gets the icon tier used for the user.
        /// </summary>
        public int IconTier
        {
            get { return m_iconTier; }
        }

        /// <summary>
        /// Gets the level of the user.
        /// </summary>
        public int Level
        {
            get { return m_level; }
        }

        /// <summary>
        /// Gets the <see cref="BNSharp.BattleNet.Product">Product</see> with which the user is currently logged on.
        /// </summary>
        public override Product Product
        {
            get { return m_prod; }
        }
        #endregion
    }
}
