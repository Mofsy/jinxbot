using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// Specifies the contract for an event handler that wishes to handle the server news event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ServerNewsEventHandler(object sender, ServerNewsEventArgs e);

    /// <summary>
    /// Represents a group of server news entries.
    /// </summary>
    public class ServerNewsEventArgs : BaseEventArgs
    {
        private List<NewsEntry> m_entries;

        internal ServerNewsEventArgs(List<NewsEntry> entries)
        {
            Debug.Assert(entries != null);

            m_entries = entries;
        }

        /// <summary>
        /// Gets an array of the news entries associated with the event.
        /// </summary>
        public NewsEntry[] Entries
        {
            get { return m_entries.ToArray(); }
        }
    }
}
