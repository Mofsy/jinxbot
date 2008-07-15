using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Represents a Battle.net Clan member.
    /// </summary>
    /// <remarks>
    /// <para>This class cannot be directly instantiated.  Rather, it is provided when you log on via the <see>TODO</see> event.</para>
    /// </remarks>
    [Serializable]
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ClanMember
    {
        #region fields
#if !NET_2_ONLY
        [DataMember(Name = "Username")]
#endif
        private string m_userName;
#if !NET_2_ONLY
        [DataMember(Name = "Rank")]
#endif
        private ClanRank m_rank;
#if !NET_2_ONLY
        [DataMember(Name = "OnlineStatus")]
#endif
        private ClanMemberStatus m_online;
#if !NET_2_ONLY
        [DataMember(Name = "Location")]
#endif
        private string m_location;
        #endregion

        #region constructors
        internal ClanMember(string userName, ClanRank rank)
        {
            m_userName = userName;
            m_rank = rank;
        }

        internal ClanMember(string userName, ClanRank rank, ClanMemberStatus status, string location)
            : this(userName, rank)
        {
            m_online = status;
            m_location = location;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the user's name.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Username</c>.</para>
        /// </remarks>
        public string Username
        {
            get { return m_userName; }
        }

        /// <summary>
        /// Gets, and in derived classes sets, the user's current <see cref="ClanRank">rank</see> within the clan.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Rank</c>.</para>
        /// </remarks>
        public ClanRank Rank
        {
            get { return m_rank; }
            protected internal set
            {
                m_rank = value;
            } 
        }

        /// <summary>
        /// Gets, and in derived classes sets, the current location and status of the clan member.
        /// </summary>
        /// <remarks>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>OnlineStatus</c>.</para>
        /// </remarks>
        public ClanMemberStatus CurrentStatus
        {
            get { return m_online; }
            protected internal set
            {
                m_online = value;
            }
        }

        /// <summary>
        /// Gets, and in derived classes sets, the user's current Battle.net location, if the user is online.
        /// </summary>
        /// <remarks>
        /// <para>This property will return <see langword="null" /> if the user is not online.</para>
        /// <para>When exposed under a WCF data contract, this property's backing store is given the name <c>Location</c>.</para>
        /// </remarks>
        public string Location
        {
            get { return m_location; }
            protected internal set
            {
                m_location = value;
            } 
        }
        #endregion
    }
}
