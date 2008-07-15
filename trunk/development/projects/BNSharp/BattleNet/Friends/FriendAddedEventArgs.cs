using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Friends
{
    /// <summary>
    /// Specifies that a new friend was added to the list of friends.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class FriendAddedEventArgs : BaseEventArgs
    {
#if !NET_2_ONLY
        [DataMember]
#endif
        private FriendUser m_newFriend;

        /// <summary>
        /// Creates a new <see>FriendAddedEventArgs</see>.
        /// </summary>
        /// <param name="newFriend">The friend that was added to the list.</param>
        public FriendAddedEventArgs(FriendUser newFriend)
        {
            m_newFriend = newFriend;
        }

        /// <summary>
        /// Gets a reference to the friend that was added.
        /// </summary>
        public FriendUser NewFriend
        {
            get { return m_newFriend; }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for friend added events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void FriendAddedEventHandler(object sender, FriendAddedEventArgs e);
}
