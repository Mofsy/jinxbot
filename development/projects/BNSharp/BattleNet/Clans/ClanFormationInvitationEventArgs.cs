﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the event arguments provided from Battle.net when the client is invited to join a new clan.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanFormationInvitationEventArgs : BaseEventArgs
    {
        #region fields
        #if !NET_2_ONLY
        [DataMember(Name = "RequestID")]
#endif
        private int m_cookie;
#if !NET_2_ONLY
        [DataMember(Name = "Tag")]
#endif
        private string m_tag;
#if !NET_2_ONLY
        [DataMember(Name = "Name")]
#endif
        private string m_clanName;
#if !NET_2_ONLY
        [DataMember(Name = "Inviter")]
#endif
        private string m_inviterName;
#if !NET_2_ONLY
        [DataMember(Name = "InvitedUsers")]
#endif
        private string[] m_invitedUsers;
        #endregion

        /// <summary>
        /// Creates a new <see>ClanFormationInvitationEventArgs</see>.
        /// </summary>
        /// <param name="requestNumber">The unique ID of the request.</param>
        /// <param name="tag">The clan tag.</param>
        /// <param name="clanName">The full name of the new clan.</param>
        /// <param name="inviter">The user responsible for the invitation.</param>
        /// <param name="invitees">The users being invited.</param>
        public ClanFormationInvitationEventArgs(int requestNumber, string tag, string clanName, string inviter, string[] invitees)
        {
            m_cookie = requestNumber;
            m_tag = tag;
            m_clanName = clanName;
            m_inviterName = inviter;
            m_invitedUsers = invitees;
        }

        /// <summary>
        /// Gets the unique ID of the request.
        /// </summary>
        /// <remarks>
        /// <para>This value should be used in the response.</para>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>RequestID</c>.</para>
        /// </remarks>
        public int RequestID
        {
            get { return m_cookie; }
        }

        /// <summary>
        /// Gets the Tag of the clan being formed.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Tag</c>.</para>
        /// </remarks>
        public string ClanTag
        {
            get { return m_tag; }
        }

        /// <summary>
        /// Gets the full name of the clan being formed.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Name</c>.</para>
        /// </remarks>
        public string ClanName
        {
            get { return m_clanName; }
        }

        /// <summary>
        /// Gets the screen name of the user sending the invitation.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Inviter</c>.</para>
        /// </remarks>
        public string InvitingUser
        {
            get { return m_inviterName; }
        }

        /// <summary>
        /// Gets a copy of the list of users being invited to join.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>InvitedUsers</c>.</para>
        /// </remarks>
        public string[] InvitedUsers
        {
            get
            {
                string[] copy = new string[m_invitedUsers.Length];
                Array.Copy(m_invitedUsers, copy, copy.Length);
                return copy;
            }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for clan formation invitation events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanFormationInvitationEventHandler(object sender, ClanFormationInvitationEventArgs e);
}
