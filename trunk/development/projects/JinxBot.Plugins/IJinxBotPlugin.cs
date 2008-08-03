using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins
{
    /// <summary>
    /// When implemented, allows a plugin to handle startup and shutdown, and persist settings.
    /// </summary>
    public interface IJinxBotPlugin : IDisposable
    {
        /// <summary>
        /// Causes a plugin to load its settings from a dictionary.
        /// </summary>
        /// <param name="settings">A dictionary containing the settings persisted from previous loads.</param>
        void Startup(IDictionary<string, string> settings);

        /// <summary>
        /// Causes a plugin to save its settings to a dictionary.
        /// </summary>
        /// <param name="settings"></param>
        void Shutdown(IDictionary<string, string> settings);
    }
}
