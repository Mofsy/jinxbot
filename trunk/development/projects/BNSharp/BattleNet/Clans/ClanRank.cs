using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet.Clans
{
    /// <summary>
    /// Specifies the ranks a clan member may have within a clan.
    /// </summary>
    public enum ClanRank
    {
        /// <summary>
        /// Specifies that the member is a new recruit who has been with the clan less than one week.
        /// </summary>
        Initiate = 0, 
        /// <summary>
        /// Specifies that the member is a new recruit who has been with the clan at least one week.
        /// </summary>
        Peon = 1,
        /// <summary>
        /// Specifies that the member is a regular clan member.
        /// </summary>
        Grunt = 2,
        /// <summary>
        /// Specifies that the member is a clan officer.
        /// </summary>
        Shaman = 3,
        /// <summary>
        /// Specifies that the member is the clan leader.
        /// </summary>
        Chieftan = 4,
    }
}
