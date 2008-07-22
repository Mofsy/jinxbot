using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Friends
{
    /// <summary>
    /// Provides a friend list received from the server.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class FriendListReceivedEventArgs : BaseEventArgs
    {
#if !NET_2_ONLY
        [DataMember(Name = "Friends")]
#endif
        private FriendUser[] m_friends;

        /// <summary>
        /// Creates a new <see>FriendListReceivedEventArgs</see>.
        /// </summary>
        /// <param name="friends">The list of friends received from Battle.net.</param>
        public FriendListReceivedEventArgs(FriendUser[] friends)
        {
            m_friends = friends;
        }

        /// <summary>
        /// Gets a copy of the friends list received from Battle.net.
        /// </summary>
        /// <remarks>
        /// <para>When this property's backing store is serialized as part of a WCF data contract,
        /// it is given the name <c>Friends</c>.</para>
        /// </remarks>
        public FriendUser[] Friends
        {
            get
            {
                FriendUser[] copy = new FriendUser[m_friends.Length];
                Array.Copy(m_friends, copy, copy.Length);
                return copy;
            }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for friend list received events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void FriendListReceivedEventHandler(object sender, FriendListReceivedEventArgs e);
}
