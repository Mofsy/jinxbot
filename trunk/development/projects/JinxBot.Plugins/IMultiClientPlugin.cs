using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        void CreatePluginWindows(object configurationWindowManager);
    }
}
