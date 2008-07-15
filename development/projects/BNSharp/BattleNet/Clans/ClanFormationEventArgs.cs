using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies information provided by Battle.net when the client attempts to form a clan.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanFormationEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember(Name="Succeeded")]
#endif
        private bool m_success;
#if !NET_2_ONLY
        [DataMember(Name="Declined")]
#endif
        private bool m_declined;
#if !NET_2_ONLY
        [DataMember(Name = "Unavailable")]
#endif
        private bool m_unavailable;
#if !NET_2_ONLY
        [DataMember(Name = "FailedAccounts")]
#endif
        private string[] m_failedAccounts;
        #endregion

        /// <summary>
        /// Creates a new <see>ClanFormationEventArgs</see> indicating success.
        /// </summary>
        public ClanFormationEventArgs()
        {
            m_success = true;
            m_failedAccounts = new string[0];
        }

        /// <summary>
        /// Creates a new <see>ClanFormationEventArgs</see> indicating failure.
        /// </summary>
        /// <param name="declined">Whether the failure is due to users declining.</param>
        /// <param name="unavailable">Whether the failure is due to users being unavailable.</param>
        /// <param name="failedAccounts">The accounts that failed.</param>
        public ClanFormationEventArgs(bool declined, bool unavailable, string[] failedAccounts)
        {
            m_declined = declined;
            m_unavailable = unavailable;
            m_failedAccounts = failedAccounts;
        }

        /// <summary>
        /// Gets whether the invitation succeeded.
        /// </summary>
        /// <remarks>
        /// <para>When this property returns <see langword="true" />, the <see>FailureAccountNames</see> property will return a zero-length array.</para>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Succeeded</c>.</para>
        /// </remarks>
        public bool Succeeded
        {
            get { return m_success; }
        }

        /// <summary>
        /// Gets whether the invitation failed because users declined.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Declined</c>.</para>
        /// </remarks>
        public bool UsersDeclined
        {
            get { return m_declined; }
        }

        /// <summary>
        /// Gets whether the invitation failed because users were unavailable.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Unavailable</c>.</para>
        /// </remarks>
        public bool UsersWereUnavailable
        {
            get { return m_unavailable; }
        }

        /// <summary>
        /// Gets a copy of the account names that failed being invited.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>FailedAccounts</c>.</para>
        /// </remarks>
        public string[] FailureAccountNames
        {
            get
            {
                string[] copy = new string[m_failedAccounts.Length];
                Array.Copy(m_failedAccounts, copy, copy.Length);
                return copy;
            }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for clan formation events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanFormationEventHandler(object sender, ClanFormationEventArgs e);
}
