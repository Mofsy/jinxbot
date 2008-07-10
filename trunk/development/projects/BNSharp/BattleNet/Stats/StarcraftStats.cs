using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.MBNCSUtil;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// Represents statistics logged onto Battle.net with Starcraft, Japan Starcraft, Warcraft II: Battle.net Edition, 
    /// or the original Diablo.
    /// </summary>
    /// <remarks>
    /// <para>This class cannot be instantiated directly.  To obtain an instance of this class, use 
    /// <see cref="UserStats.Parse">UserStats.Parse</see>, and cast the result to this class.</para>
    /// </remarks>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class StarcraftStats : UserStats
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private Product m_prod;

#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_ladderRating;
#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_ladderRank;
#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_wins;
#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_highestLadderRating;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_iconCode;
#if !NET_2_ONLY
        [DataMember]
#endif
        private bool m_isSpawned;
        #endregion

        #region Constructor
        internal StarcraftStats(byte[] stats)
        {
            // RATS 0 0 200 0 0 0 0 0 RATS
            // pcode rating rank wins spawn unknown hirating unkn unkn icon
            DataReader dr = new DataReader(stats);
            string productCode = dr.ReadDwordString(0);
            m_prod = Product.GetByProductCode(productCode);
            if (m_prod == null)
                m_prod = Product.UnknownProduct;

            dr.ReadTerminatedString(' ', Encoding.ASCII);
            string sRating = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sRating, out m_ladderRating);
            string sRank = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sRank, out m_ladderRank);
            string sWins = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sWins, out m_wins);
            int nSpawn;
            string sSpawn = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sSpawn, out nSpawn);
            m_isSpawned = (nSpawn == 1);
            dr.ReadTerminatedString(' ', Encoding.ASCII);
            string sHighRating = dr.ReadTerminatedString(' ', Encoding.ASCII);
            int.TryParse(sHighRating, out m_highestLadderRating);
            dr.ReadTerminatedString(' ', Encoding.ASCII);
            dr.ReadTerminatedString(' ', Encoding.ASCII);
            m_iconCode = dr.ReadDwordString(0);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether the user is logged on with a spawned client.
        /// </summary>
        public bool IsSpawn
        {
            get { return m_isSpawned; }
        }

        /// <summary>
        /// Gets the icon code for the user issued by Battle.net.
        /// </summary>
        public string IconCode
        {
            get { return m_iconCode; }
        }

        /// <summary>
        /// Gets the user's highest ladder rating.
        /// </summary>
        public int HighestLadderRating
        {
            get { return m_highestLadderRating; }
        }

        /// <summary>
        /// Gets the user's win count.
        /// </summary>
        public int Wins
        {
            get { return m_wins; }
        }

        /// <summary>
        /// Gets the user's current ladder rank.
        /// </summary>
        public int LadderRank
        {
            get { return m_ladderRank; }
        }

        /// <summary>
        /// Gets the user's current ladder rating.
        /// </summary>
        public int LadderRating
        {
            get { return m_ladderRating; }
        }

        /// <summary>
        /// Gets the <see cref="BNSharp.BattleNet.Product">Product</see> with which the user is logged on.
        /// </summary>
        public override Product Product
        {
            get { return m_prod; }
        }
        #endregion
    }
}
