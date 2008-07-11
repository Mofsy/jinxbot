using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Contains information about an attempt to disband the client's clan.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanDisbandEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private bool m_succeeded;
        #endregion

        /// <summary>
        /// Creates a new <see>ClanDisbandEventArgs</see>.
        /// </summary>
        /// <param name="disbandSucceeded">Whether the disband succeeded.</param>
        public ClanDisbandEventArgs(bool disbandSucceeded)
        {
            m_succeeded = disbandSucceeded;
        }

        /// <summary>
        /// Gets whether the disband succeeded.
        /// </summary>
        /// <remarks>
        /// <para>The disband may fail if the client is not the clan leader, or if the clan is not at least one week old.</para>
        /// </remarks>
        public bool DisbandSucceeded
        {
            get { return m_succeeded; }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for clan disband events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanDisbandEventHandler(object sender, ClanDisbandEventArgs e);
}
