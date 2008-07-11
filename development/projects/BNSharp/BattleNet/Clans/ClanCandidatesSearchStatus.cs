using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the status of a clan candidates search.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public enum ClanCandidatesSearchStatus
    {
        /// <summary>
        /// Indicates that the search was a success and that the tag is available.
        /// </summary>
        Success,
        /// <summary>
        /// Indicates that the requested tag is already taken.
        /// </summary>
        ClanTagTaken,
        /// <summary>
        /// Indicates that the client user is already in a clan.
        /// </summary>
        AlreadyInClan,
        /// <summary>
        /// Specifies the tag requested was invalid.
        /// </summary>
        InvalidTag,
    }
}
