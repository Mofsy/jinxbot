using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// The list of races that are supported by Warcraft III for Battle.net.
    /// </summary>
    public enum Warcraft3IconRace
    {
        /// <summary>
        /// Specifies that the icon race sent from Battle.net was unrecognized.
        /// </summary>
        Unknown,
        /// <summary>
        /// Specifies that a user's icon is based on the random list.
        /// </summary>
        Random,
        /// <summary>
        /// Specifies that a user's icon is based on the tournament list.
        /// </summary>
        Tournament,
        /// <summary>
        /// Specifies that a user's icon is based on the human list.
        /// </summary>
        Human,
        /// <summary>
        /// Specifies that a user's icon is based on the orc list.
        /// </summary>
        Orc,
        /// <summary>
        /// Specifies that a user's icon is based on the night elf list.
        /// </summary>
        NightElf,
        /// <summary>
        /// Specifies that a user's icon is based on the undead list.
        /// </summary>
        Undead,
    }
}
