using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// The event arguments for the clan member list notification.
    /// </summary>
    [Serializable]
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanMemberListEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private ClanMember[] m_members;
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ClanMemberListEventArgs</see>.
        /// </summary>
        /// <param name="members">The clan members in the list.</param>
        public ClanMemberListEventArgs(ClanMember[] members)
        {
            m_members = members;
        }

        /// <summary>
        /// Gets the list of members received from Battle.net.
        /// </summary>
        public ClanMember[] Members
        {
            get { return m_members; }
        }
    }

    /// <summary>
    /// Specifies the contract for clan member list events.
    /// </summary>
    /// <param name="sender">The <see>BattleNetClient</see> connection that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanMemberListEventHandler(object sender, ClanMemberListEventArgs e);
}
