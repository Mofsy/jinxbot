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

        /// <summary>
        /// Causes a plugin to check whether it should be updated.
        /// </summary>
        /// <remarks>
        /// <para>During initialization and plugin discovery, JinxBot prompts all located plugins to determine whether they should be updated.  A plugin
        /// that does not support this capability should simply return <see langword="false" /> in all cases.</para>
        /// </remarks>
        /// <returns><see langword="true" /> if the plugin should be updated; otherwise <see langword="false" />.</returns>
        bool CheckForUpdates();
    }
}
