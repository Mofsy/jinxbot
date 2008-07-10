using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for chat events that involve another user, but not specifically communication.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void UserEventHandler(object sender, UserEventArgs e);

    /// <summary>
    /// Specifies event information for chat events that involve another user, but not specifically communication.
    /// </summary>
    /// <para>An example of when this class would be used is for a user joined or user left event.</para>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class UserEventArgs : ChatEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_ping;
#if !NET_2_ONLY
        [DataMember]
#endif
        private UserFlags m_flags;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_un;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_txt;
#if !NET_2_ONLY
        [DataMember]
#endif
        private byte[] m_statBytes; 
        #endregion

        /// <summary>
        /// Creates a new <see>UserEventArgs</see> with the specified settings.
        /// </summary>
        /// <param name="eventType">The type of chat event.</param>
        /// <param name="flags">The user-specific flags.</param>
        /// <param name="ping">The user's latency to Battle.net.</param>
        /// <param name="username">The user's display name.</param>
        /// <param name="stats">The user's client information.</param>
        public UserEventArgs(ChatEventType eventType, UserFlags flags, int ping, string username, byte[] stats)
            : base(eventType)
        {
            m_flags = flags;
            m_ping = ping;
            m_un = username;
            m_statBytes = stats;

            m_txt = Encoding.ASCII.GetString(stats);
        }

        /// <summary>
        /// Gets the user's latency to Battle.net.
        /// </summary>
        public int Ping
        {
            get { return m_ping; }
        }

        /// <summary>
        /// Gets user-specific flags.
        /// </summary>
        public UserFlags Flags
        {
            get { return m_flags; }
        }

        /// <summary>
        /// Gets the user's display name.
        /// </summary>
        public string Username
        {
            get { return m_un; }
        }

        /// <summary>
        /// Gets the user's client information string, which typically contains gameplay statistics and client identifier.
        /// </summary>
        public string Statstring
        {
            get { return m_txt; }
        }

        /// <summary>
        /// Gets the literal user stats data.
        /// </summary>
        public byte[] StatsData
        {
            get
            {
                byte[] data = new byte[m_statBytes.Length];
                Buffer.BlockCopy(m_statBytes, 0, data, 0, m_statBytes.Length);
                return data;
            }
        }
    }
}
