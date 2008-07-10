using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Contains information about when the client's user's clan rank has changed.
    /// </summary>
    public class ClanRankChangeEventArgs : BaseEventArgs
    {
        private ClanRank m_old, m_new;
        private ClanMember m_changer;

        internal ClanRankChangeEventArgs(ClanRank oldRank, ClanRank newRank, ClanMember memberWhoChangedTheRank)
        {
            m_old = oldRank;
            m_new = newRank;
            m_changer = memberWhoChangedTheRank;
        }

        /// <summary>
        /// Gets your previous clan rank.
        /// </summary>
        public ClanRank PreviousRank
        {
            get { return m_old; }
        }

        /// <summary>
        /// Gets your new clan rank.
        /// </summary>
        public ClanRank NewRank
        {
            get { return m_new; }
        }

        /// <summary>
        /// Gets the <see>ClanMember</see> who changed the rank.
        /// </summary>
        public ClanMember MemberResponsible
        {
            get { return m_changer; }
        }
    }

    /// <summary>
    /// Specifies the contract for when handlers want to take care of the event in which the client's clan rank has changed.
    /// </summary>
    /// <param name="sender">The <see>BattleNetClient</see> that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanRankChangeEventHandler(object sender, ClanRankChangeEventArgs e);
}
