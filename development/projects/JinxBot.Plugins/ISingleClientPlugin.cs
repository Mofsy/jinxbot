using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;
using BNSharp.BattleNet;

namespace JinxBot.Plugins
{
    
    public interface ISingleClientPlugin : IJinxBotPlugin
    {
        /// <summary>
        /// Causes the plugin to create tabs or tool windows within the profile window.
        /// </summary>
        /// <param name="profileDocument">A <see cref="IProfileDocument">profile document interface</see> that provides access to creating
        /// additional windows for the profile.</param>
        void CreatePluginWindows(IProfileDocument profileDocument);
        /// <summary>
        /// Releases the plugin windows created during startup of the plugin.
        /// </summary>
        /// <param name="profileDocument">A <see cref="IProfileDocument">profile document interface</see>
        /// that provides access to creating additional windows.</param>
        void DestroyPluginWindows(IProfileDocument profileDocument);

        /// <summary>
        /// Instructs the plugin to register for events from the client.
        /// </summary>
        /// <param name="profileClient">The related client.</param>
        void RegisterEvents(IJinxBotClient profileClient);
        /// <summary>
        /// Instructs the plugin to unregister for events from the client.
        /// </summary>
        /// <param name="profileClient">The related client.</param>
        void UnregisterEvents(IJinxBotClient profileClient);
    }
}
