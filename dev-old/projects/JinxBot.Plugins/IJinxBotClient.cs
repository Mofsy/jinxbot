using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;
using JinxBot.Plugins.UI;
using JinxBot.Plugins.Data;

namespace JinxBot.Plugins
{
    /// <summary>
    /// Provides access to the components of a single JinxBot client profile.
    /// </summary>
    public interface IJinxBotClient
    {
        /// <summary>
        /// Gets the <see>BattleNetClient</see> representing the client that will be used to connect to 
        /// Battle.net.
        /// </summary>
        BattleNetClient Client { get; }
        /// <summary>
        /// Gets a reference to the main window representing this profile.
        /// </summary>
        /// <remarks>
        /// <para>This object reference is guaranteed to be able to be cast to <see>IProfileDocument</see>.</para>
        /// <para>This property may be changed to return an interface that inherits from both
        /// <see>IChatTab</see> and IProfileDocument in future releases.</para>
        /// </remarks>
        IChatTab MainWindow { get; }
        /// <summary>
        /// Gets access to the <see>IJinxBotDatabase</see> database implementation.  By default, JinxBot 
        /// provides a basic database that assigns no roles to any user except the currently-connected 
        /// user.
        /// </summary>
        IJinxBotDatabase Database { get; }

        /// <summary>
        /// Sends a message to Battle.net if the client is currently connected.
        /// </summary>
        /// <remarks>
        /// <para>This is a convenience method for plugins that calls immediately into 
        /// <see cref="M:BNSharp.BattleNet.BattleNetClient.SendMessage(System.String)">BattleNetClient.Send</see>.</para>
        /// </remarks>
        /// <param name="message">The message to send to Battle.net.</param>
        void SendMessage(string message);
    }
}
