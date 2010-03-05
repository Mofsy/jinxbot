using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;

namespace JinxBot.Plugins
{
    /// <summary>
    /// Represents a plugin that should respond and handle multiple clients, such as a moderation client.
    /// </summary>
    /// <remarks>
    /// <para>This interface is incompatible with <see>ISingleClientPlugin</see>; both may be applied to a class, and may be loaded into the 
    /// same profile, but there will be separate instances of the class loaded to handle single-client and multi-client service requests.</para>
    /// </remarks>
    public interface IMultiClientPlugin : IJinxBotPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationWindowManager"></param>
        void CreatePluginWindows(IMainWindow configurationWindowManager);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationWindowManager"></param>
        void DestroyPluginWindows(IMainWindow configurationWindowManager);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        void AddClient(IJinxBotClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        void RemoveClient(IJinxBotClient client);

    }
}
