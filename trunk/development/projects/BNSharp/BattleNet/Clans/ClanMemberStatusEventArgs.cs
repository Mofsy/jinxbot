using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the event arguments for a clan member's status change event.
    /// </summary>
    public class ClanMemberStatusEventArgs : BaseEventArgs
    {
        private ClanMember m_member;

        internal ClanMemberStatusEventArgs(ClanMember associatedMember)
        {
            m_member = associatedMember;
        }

        /// <summary>
        /// Gets the associated clan member.
        /// </summary>
        public ClanMember Member
        {
            get { return m_member; }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers of clan member status events.
    /// </summary>
    /// <param name="sender">The <see>BattleNetClient</see> that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanMemberStatusEventHandler(object sender, ClanMemberStatusEventArgs e);
}
