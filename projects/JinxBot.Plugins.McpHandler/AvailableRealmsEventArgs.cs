using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp;

namespace JinxBot.Plugins.McpHandler
{
    [Serializable]
    public class AvailableRealmsEventArgs : BaseEventArgs
    {
        private RealmServer[] m_realms;

        public AvailableRealmsEventArgs(RealmServer[] realms)
        {
            m_realms = realms;
        }

        /// <summary>
        /// Gets a copy of the list of available realms.
        /// </summary>
        public RealmServer[] Realms
        {
            get
            {
                RealmServer[] copy = new RealmServer[m_realms.Length];
                Array.Copy(m_realms, copy, copy.Length);
                return copy;
            }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for realm list retrieval events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void AvailableRealmsEventHandler(object sender, AvailableRealmsEventArgs e);
}
