using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// The event arguments for the clan member list notification.
    /// </summary>
    public class ClanMemberListEventArgs : BaseEventArgs
    {
        private ClanMember[] m_members;

        internal ClanMemberListEventArgs(ClanMember[] members)
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
