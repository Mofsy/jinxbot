using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.McpHandler
{
    [Serializable]
    public class RealmConnectedEventArgs
    {
        private RealmConnectionStatus m_status;

        public RealmConnectedEventArgs(RealmConnectionStatus status)
        {
            m_status = status;
        }

        public RealmConnectionStatus Status
        {
            get { return m_status; }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for realm connected events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void RealmConnectedEventHandler(object sender, RealmConnectedEventArgs e);
}
