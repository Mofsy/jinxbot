using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;

namespace JinxBot.Plugins
{
    
    public interface ISingleClientPlugin : IJinxBotPlugin
    {
        void CreatePluginWindows(IProfileDocument profileDocument);
        /// <summary>
        /// Releases the plugin windows created during startup of the plugin.
        /// </summary>
        /// <param name="profileDocument">A <see cref="IProfileDocument">profile document interface</see>
        /// that provides access to creating additional windows.</param>
        void DestroyPluginWindows(IProfileDocument profileDocument);
    }
}
