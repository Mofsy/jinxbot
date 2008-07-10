using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the flags that can be related to user-specific <see cref="UserEventArgs.Flags">chat events</see>.
    /// </summary>
    [Flags]
#if !NET_2_ONLY
    [DataContract]
#endif
    public enum UserFlags
    {
        /// <summary>
        /// Indicates that the user is normal.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the user is a Blizzard representative.
        /// </summary>
        BlizzardRepresentative = 1,
        /// <summary>
        /// Indicates that the user is a channel operator.
        /// </summary>
        ChannelOperator = 2,
        /// <summary>
        /// Indicates that the user has a Speaker icon.
        /// </summary>
        Speaker = 4,
        /// <summary>
        /// Indicates that the user is a Battle.net Administrator.
        /// </summary>
        BattleNetAdministrator = 8,
        /// <summary>
        /// Indicates that the user's client expects UDP support but lacks it.
        /// </summary>
        NoUDP = 0x10,
        /// <summary>
        /// Indicates that the user is currently squelched by the client.
        /// </summary>
        Squelched = 0x20,
        /// <summary>
        /// Indicates a channel special guest.
        /// </summary>
        SpecialGuest = 0x40,
        /// <summary>
        /// Represented when the client had "beep" enabled, a client-side setting.  No longer supported on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        BeepEnabled = 0x100,
        /// <summary>
        /// Represented PGL players.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        PglPlayer = 0x200,
        /// <summary>
        /// Represented PGL officials.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        PglOfficial = 0x400,
        /// <summary>
        /// Represented KBK players.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        KbkPlayer = 0x800,
        /// <summary>
        /// The flag for WCG officials.
        /// </summary>
        WcgOfficial = 0x1000,
        /// <summary>
        /// Represented KBK singles players.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        KbkSingles = 0x2000,
        /// <summary>
        /// Represented beginner KBK players.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        KbkBeginner = 0x10000,
        /// <summary>
        /// Represented a single bar for KBK players.  No longer seen on Battle.net.
        /// </summary>
        [Obsolete("No longer seen on Battle.net.  For more information see http://www.bnetdocs.org/?op=doc&did=15.", false)]
        WhiteKbk = 0x20000,
        /// <summary>
        /// The flag for GF officials.
        /// </summary>
        GFOfficial = 0x100000,
        /// <summary>
        /// The flag for GF players.
        /// </summary>
        GFPlayer = 0x200000,
        /// <summary>
        /// The current flag for PGL players.
        /// </summary>
        PglPlayer2 = 0x2000000,
    }
}
