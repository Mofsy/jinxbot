using BNSharp.BattleNet.Stats;
using BNSharp.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
/// <summary>
    /// Represents a user found in a channel.
    /// </summary>
    public class ChatUser : IChatUser
    {
        /// <summary>
        /// Creates a new <see>ChatUser</see>.
        /// </summary>
        /// <param name="userName">Specifies the user's fully-qualified username, including possibly the character name, 
        /// name separator (for Diablo 2 characters), and realm namespace qualifier.</param>
        /// <param name="ping">The user's latency.</param>
        /// <param name="flags">The user's flags.</param>
        /// <param name="stats">The user's stats.</param>
        /// <remarks>
        /// <para>The user's stats can be determined by passing the username and binary statsring value to 
        /// <see cref="UserStats.Parse">UserStats.Parse</see>.</para>
        /// </remarks>
        public ChatUser(string userName, int ping, ClassicUserFlags flags, UserStats stats)
        {
            Username = userName;
            Ping = ping;
            Flags = flags;
            Stats = stats;
        }

        /// <summary>
        /// Gets, and in derived classes sets, the user's latency to Battle.net.
        /// </summary>
        public int Ping
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets, and in derived classes sets, user-specific flags.
        /// </summary>
        public ClassicUserFlags Flags
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the user's full display name.
        /// </summary>
        public string Username
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the user's stats.
        /// </summary>
        /// <remarks>
        /// <para>For more information about the user's stats, you should check the <see cref="UserStats.Product">Product</see>
        /// property of the object and then cast to one of the descendant classes.  For more information, see 
        /// <see>UserStats</see>.</para>
        /// </remarks>
        /// <seealso cref="UserStats"/>
        /// <seealso cref="Warcraft3Stats"/>
        /// <seealso cref="Diablo2Stats"/>
        /// <seealso cref="StarcraftStats"/>
        /// <seealso cref="DefaultStats"/>
        public UserStats Stats
        {
            get;
            internal set;
        }

    }
}
