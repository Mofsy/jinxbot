﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// Contains information about a Battle.net gateway, including information about how it renders chat usernames.
    /// </summary>
    [DebuggerDisplay("{_name} Gateway: {_serverName}:{_port}")]
    public sealed class Gateway
    {
        #region Instance fields
        private string _oldClientSuffix;
        private string _war3Suffix;
        private string _serverName;
        private int _port;
        private string _name;
        #endregion

        #region Static fields
        // This class is immutable, so they may be captured using static readonly fields instead of singleton properties.

        /// <summary>
        /// Gets a <see>Gateway</see> representing the official Battle.net US East server.
        /// </summary>
        public static readonly Gateway USEast = new Gateway("US East", "@USEast", "@Azeroth", "useast.battle.net", 6112);
        /// <summary>
        /// Gets a <see>Gateway</see> representing the official Battle.net US West server.
        /// </summary>
        public static readonly Gateway USWest = new Gateway("US West", "@USWest", "@Lordaeron", "uswest.battle.net", 6112);
        /// <summary>
        /// Gets a <see>Gateway</see> representing the official Battle.net Europe server.
        /// </summary>
        public static readonly Gateway Europe = new Gateway("Europe", "@Europe", "@Kalimdor", "europe.battle.net", 6112);
        /// <summary>
        /// Gets a <see>Gateway</see> representing the official Battle.net Asia server.
        /// </summary>
        public static readonly Gateway Asia = new Gateway("Asia", "@Asia", "@Northrend", "asia.battle.net", 6112);
        #endregion

        /// <summary>
        /// Creates a new <see>Gateway</see>.
        /// </summary>
        /// <param name="name">The descriptive name of the gatway.</param>
        /// <param name="oldClientSuffix">The suffix used for old clients when viewed by Warcraft 3 (see
        /// <see>OldClientSuffix</see> for more information).</param>
        /// <param name="war3ClientSuffix">The suffix used for Warcraft 3 clients when viewed by older clients (see
        /// <see>Warcraft3ClientSuffix</see> for more information).</param>
        /// <param name="serverName">The DNS host name or string representation of the IP address of the server.</param>
        /// <param name="port">The port to which to connect.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serverName"/> is <see langword="null" />
        /// or zero-length.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="port"/> is less than 1 or greater than
        /// 65535.</exception>
        public Gateway(string name, string oldClientSuffix, string war3ClientSuffix, string serverName, int port)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(serverName))
                throw new ArgumentNullException("serverName");
            if (port <= 0 || port > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("port", port, "Valid values for the port parameter are 1 to 65535.");

            _name = name;
            _oldClientSuffix = oldClientSuffix;
            _war3Suffix = war3ClientSuffix;
            _serverName = serverName;
            _port = port;
        }

        #region instance properties
        /// <summary>
        /// Gets the old client suffix, e.g., <c>@USEast</c>, for the specified gateway.
        /// </summary>
        public string OldClientSuffix
        {
            get
            {
                return _oldClientSuffix;
            }
        }

        /// <summary>
        /// Gets the Warcraft 3 client suffix, e.g., <c>@Azeroth</c>, for the specified gateway.
        /// </summary>
        public string Warcraft3ClientSuffix
        {
            get { return _war3Suffix; }
        }

        /// <summary>
        /// Gets the DNS host name, e.g., <c>useast.battle.net</c>, for the specified gateway.
        /// </summary>
        public string ServerHost
        {
            get { return _serverName; }
        }

        /// <summary>
        /// Gets the server port associated with this gateway.
        /// </summary>
        [DefaultValue(6112)]
        public int ServerPort
        {
            get { return _port; }
        }

        /// <summary>
        /// Gets the descriptive name of this gateway.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentUICulture, "\"{0}\", {1}:{2}", _name, _serverName, _port);
        }
    }
}
