using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Configuration
{
    /// <summary>
    /// Specifies the types of icon configurations that JinxBot supports.
    /// </summary>
    public enum IconType
    {
        /// <summary>
        /// Specifies that the system should use the StarCraft Icons.bni file.
        /// </summary>
        IconsBni,
        /// <summary>
        /// Specifies that the system should download the icons from the Battle.net web site.
        /// </summary>
        BattleNetWebSite
    }
}
