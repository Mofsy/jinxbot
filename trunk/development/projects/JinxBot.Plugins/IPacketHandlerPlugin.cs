using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;

namespace JinxBot.Plugins
{
    /// <summary>
    /// When implemented by a plugin, allows the plugin to use custom BN# packet handling routines.
    /// </summary>
    public interface IPacketHandlerPlugin : ISingleClientPlugin
    {
        /// <summary>
        /// When called by JinxBot, causes the plugin to register itself for custom packet handling.
        /// </summary>
        /// <param name="client">The client connection to register.</param>
        void RegisterPackets(BattleNetClient client);

        /// <summary>
        /// When called by JinxBot, causes the plugin to unregister itself for packet handling.
        /// </summary>
        /// <param name="client">The client connection to unregister.</param>
        void UnregisterPackets(BattleNetClient client);
    }
}
