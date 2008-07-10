using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// Specifies the character classes supported by Diablo 2 characters.
    /// </summary>
    public enum Diablo2CharacterClass
    {
        /// <summary>
        /// Specifies that the class is unknown or invalid.
        /// </summary>
        Unknown,
        /// <summary>
        /// Specifies the Amazon class (female).
        /// </summary>
        Amazon = 1,
        /// <summary>
        /// Specifies the Sorceress class (female).
        /// </summary>
        Sorceress = 2,
        /// <summary>
        /// Specifies the Necromancer class (male).
        /// </summary>
        Necromancer = 3,
        /// <summary>
        /// Specifies the Paladin class (male).
        /// </summary>
        Paladin = 4,
        /// <summary>
        /// Specifies the Barbarian class (male).
        /// </summary>
        Barbarian = 5,
        /// <summary>
        /// Specifies the Druid class (male).
        /// </summary>
        Druid = 6,
        /// <summary>
        /// Specifies the Assassin class (female).
        /// </summary>
        Assassin = 7,
    }
}
