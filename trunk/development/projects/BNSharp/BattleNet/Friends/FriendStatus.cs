using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Friends
{
    /// <summary>
    /// Specifies additional information about a person on your friend list.
    /// </summary>
    [Flags]
#if !NET_2_ONLY
    [DataContract]
#endif
    public enum FriendStatus
    {
        /// <summary>
        /// No additional information is provided.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user also listed you as a friend.
        /// </summary>
        Mutual = 1,
        /// <summary>
        /// The user has flagged themselves as do-not-disturb.
        /// </summary>
        DoNotDisturb = 2,
        /// <summary>
        /// The user is away.
        /// </summary>
        Away = 4,
    }
}
