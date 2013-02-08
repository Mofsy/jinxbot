﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// When implemented, provides the necessary settings to establish a Battle.net client connection.
    /// </summary>
    public interface IBattleNetSettings
    {
        /// <summary>
        /// Gets or sets the client product code.
        /// </summary>
        /// <remarks>
        /// <para>There are seven clients that may be currently emulated using BN#:</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Product Name</term>
        ///         <description>Product Code</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Starcraft (Retail)</term>
        ///         <description>STAR</description>
        ///     </item>
        ///     <item>
        ///         <term>Starcraft: Brood War</term>
        ///         <description>SEXP</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft II: Battle.net Edition</term>
        ///         <description>W2BN</description>
        ///     </item>
        ///     <item>
        ///         <term>Diablo II (Retail)</term>
        ///         <description>D2DV</description>
        ///     </item>
        ///     <item>
        ///         <term>Diablo II: The Lord of Destruction</term>
        ///         <description>D2XP</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft III: The Reign of Chaos</term>
        ///         <description>WAR3</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft III: The Frozen Throne</term>
        ///         <description>W3XP</description>
        ///     </item>
        /// </list>
        /// </remarks>
        ClassicProduct Client
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version byte.
        /// </summary>
        /// <remarks>
        /// <para>The version byte is a value that cannot be reliably retrieved from game files.  It can be found on most 
        /// web sites, or is generally updated on <a href="http://www.jinxbot.net/wiki/index.php?title=Client_Versions" target="_blank">the JinxBot Wiki</a>.</para>
        /// </remarks>
        int VersionByte
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Product CD key.
        /// </summary>
        /// <remarks>
        /// <para>This property is required for all products.</para>
        /// </remarks>
        string CdKey1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Expansion Product CD key, if applicable.
        /// </summary>
        /// <remarks>
        /// <para>This property is only required when emulating Warcraft III: The Frozen Throne or Diablo II: Lord of Destruction.  However,
        /// when emulating the down-level clients, it will still be used if it is supported (for example, when logging onto Warcraft III: The 
        /// Reign of Chaos even though you have Warcraft III: The Frozen Throne installed).</para>
        /// </remarks>
        string CdKey2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to a stream containing the executable file used for revision checking.
        /// </summary>
        /// <remarks>
        /// <para>This file varies by client.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Product</term>
        ///         <description>File</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Starcraft; Starcraft: Brood War</term>
        ///         <description>Starcraft.exe</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft II: Battle.net Edition</term>
        ///         <description>Warcraft II BNE.exe</description>
        ///     </item>
        ///     <item>
        ///         <term>Diablo II; Diablo II: Lord of Destruction</term>
        ///         <description>Game.exe</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft III: The Reign of Chaos; Warcraft III: The Frozen Throne</term>
        ///         <description>War3.exe</description>
        ///     </item>
        /// </list>
        /// </remarks>
        Stream GameExe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to a stream containing the second file used for revision checking.
        /// </summary>
        /// <remarks>
        /// <para>This file varies by client.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Product</term>
        ///         <description>File</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Starcraft; Starcraft: Brood War; Warcraft II: Battle.net Edition; 
        ///         Warcraft III: The Reign of Chaos; Warcraft III: The Frozen Throne</term>
        ///         <description>Storm.dll</description>
        ///     </item>
        ///     <item>
        ///         <term>Diablo II; Diablo II: Lord of Destruction</term>
        ///         <description>Bnclient.dll</description>
        ///     </item>
        /// </list>
        /// </remarks>
        Stream GameFile2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to a stream containing the third file used for revision checking.
        /// </summary>
        /// <remarks>
        /// <para>This file varies by client.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Product</term>
        ///         <description>File</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Starcraft; Starcraft: Brood War; Warcraft II: Battle.net Edition</term>
        ///         <description>Battle.snp</description>
        ///     </item>
        ///     <item>
        ///         <term>Diablo II; Diablo II: Lord of Destruction</term>
        ///         <description>D2Client.dll</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft III: The Reign of Chaos; Warcraft III: The Frozen Throne</term>
        ///         <description>Game.dll</description>
        ///     </item>
        /// </list>
        /// </remarks>
        Stream GameFile3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to a stream containing the file used for Lockdown revision checking.
        /// </summary>
        /// <remarks>
        /// <para>This property is not required for products other than Starcraft and Warcraft II: Battle.net Edition.</para>
        /// </remarks>
        Stream ImageFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the account that should be used to connect.
        /// </summary>
        string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password of the account used to connect.
        /// </summary>
        string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Gateway that should be used to connect to the server.
        /// </summary>
        Gateway Gateway
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the CD Key Owner.
        /// </summary>
        /// <remarks>
        /// <para>This name is the one that appears if you attempt to log in with the CD key while it is already logged in.</para>
        /// </remarks>
        string CdKeyOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of ping that should be used for the connection.
        /// </summary>
        PingKind PingMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the channel the client should join when first logging in.
        /// </summary>
        /// <remarks>
        /// <para>If this property returns <see langword="null" /> or an empty string, then the standard channel
        /// join will be performed (for example, a client connecting with Starcraft: Brood War emulation might 
        /// join the channel Brood War USA-2).</para>
        /// </remarks>
        string HomeChannel
        {
            get;
            set;
        }
    }
}
