using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the result of a search for clan candidates.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanCandidatesSearchEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private ClanCandidatesSearchStatus m_status;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string[] m_candidateNames;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see>ClanCandidatesSearchEventArgs</see> for a request that was unsuccessful.
        /// </summary>
        /// <param name="status">The status reported by Battle.net.</param>
        public ClanCandidatesSearchEventArgs(ClanCandidatesSearchStatus status)
        {
            m_status = status;
            m_candidateNames = new string[0];
        }

        /// <summary>
        /// Creates a new <see>ClanCandidatesSearchEventArgs</see> for a request that was successful.
        /// </summary>
        /// <param name="status">The status reported by Battle.net.</param>
        /// <param name="candidateNames">The list of candidate names provided by Battle.net.</param>
        public ClanCandidatesSearchEventArgs(ClanCandidatesSearchStatus status, string[] candidateNames)
        {
            m_status = status;
            m_candidateNames = candidateNames;
        }
        #endregion

        /// <summary>
        /// Gets a copy of the list of candidates found in the channel.
        /// </summary>
        public string[] Candidates
        {
            get
            {
                string[] copy = new string[m_candidateNames.Length];
                Array.Copy(m_candidateNames, copy, copy.Length);
                return copy;
            }
        }

        /// <summary>
        /// Gets the functional result of the search.
        /// </summary>
        public ClanCandidatesSearchStatus Status
        {
            get { return m_status; }
        }
    }

    /// <summary>
    /// Specifies the contract for event handlers that want to listen to clan candidates search events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ClanCandidatesSearchEventHandler(object sender, ClanCandidatesSearchEventArgs e);
}
