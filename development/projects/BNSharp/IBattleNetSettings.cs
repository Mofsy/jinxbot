﻿using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.BattleNet;

namespace BNSharp
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
        string Client
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version byte.
        /// </summary>
        /// <remarks>
        /// <para>The version byte is a value that cannot be reliably retrieved from game files.  It can be </para>
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
        /// Gets or sets the full or relative path to the executable file used for revision checking.
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
        string GameExe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full or relative path to the second file used for revision checking.
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
        string GameFile2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full or relative path to the third file used for revision checking.
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
        ///         <description>D2Client.snp</description>
        ///     </item>
        ///     <item>
        ///         <term>Warcraft III: The Reign of Chaos; Warcraft III: The Frozen Throne</term>
        ///         <description>Game.dll</description>
        ///     </item>
        /// </list>
        /// </remarks>
        string GameFile3
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
        /// Gets or sets the full or relative path to the file used for Lockdown revision checking.
        /// </summary>
        /// <remarks>
        /// <para>This property is not required for products other than Starcraft and Diablo 2.</para>
        /// </remarks>
        string ImageFile
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
        /// Gets or sets the name or IP address of the server used to connect.
        /// </summary>
        string Server
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port that should be used to connect.
        /// </summary>
        /// <remarks>
        /// <para>The default port for Battle.net is 6112.</para>
        /// </remarks>
        int Port
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
        PingType PingMethod
        {
            get;
            set;
        }
    }
}
