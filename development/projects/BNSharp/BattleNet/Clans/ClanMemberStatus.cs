using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the current status of a clan member.
    /// </summary>
    [DataContract]
    public enum ClanMemberStatus
    {
        /// <summary>
        /// Specifies that the user is offline.
        /// </summary>
        Offline = 0,
        /// <summary>
        /// Specifies that the user is online but not in a channel or a game.
        /// </summary>
        Online = 1,
        /// <summary>
        /// Specifies that the user is in a channel.
        /// </summary>
        InChannel = 2,
        /// <summary>
        /// Specifies that the user is in a public game.
        /// </summary>
        InPublicGame = 3,
        /// <summary>
        /// Specifies that the user is in a private game.
        /// </summary>
        InPrivateGame = 5,
    }
}
