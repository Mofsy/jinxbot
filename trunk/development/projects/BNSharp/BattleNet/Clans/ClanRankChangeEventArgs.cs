using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Contains information about when the client's user's clan rank has changed.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanRankChangeEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember(Name = "PreviousRank")]
#endif
        private ClanRank m_old;
#if !NET_2_ONLY
        [DataMember(Name = "NewRank")]
#endif
        private ClanRank m_new;
#if !NET_2_ONLY
        [DataMember(Name = "MemberResponsible")]
#endif
        private ClanMember m_changer; 
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ClanRankChangeEventArgs</see>.
        /// </summary>
        /// <param name="oldRank">The previous rank.</param>
        /// <param name="newRank">The new rank.</param>
        /// <param name="memberWhoChangedTheRank">The member who was responsible for the rank change.</param>
        public ClanRankChangeEventArgs(ClanRank oldRank, ClanRank newRank, ClanMember memberWhoChangedTheRank)
        {
            m_old = oldRank;
            m_new = newRank;
            m_changer = memberWhoChangedTheRank;
        }

        /// <summary>
        /// Gets your previous clan rank.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>PreviousRank</c>.</para>
        /// </remarks>
        public ClanRank PreviousRank
        {
            get { return m_old; }
        }

        /// <summary>
        /// Gets your new clan rank.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>NewRank</c>.</para>
        /// </remarks>
        public ClanRank NewRank
        {
            get { return m_new; }
        }

        /// <summary>
        /// Gets the <see>ClanMember</see> who changed the rank.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>MemberResponsible</c>.</para>
        /// </remarks>
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
