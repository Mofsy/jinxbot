using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp;

namespace JinxBot.Plugins.McpHandler
{
    [Serializable]
    public class RealmFailedEventArgs : BaseEventArgs
    {
        private RealmFailureReason m_reason;

        public RealmFailedEventArgs(RealmFailureReason reason)
        {
            m_reason = reason;
        }

        public RealmFailureReason Reason
        {
            get { return m_reason; }
        }
    }

    /// <summary>
    /// Specifies the contract for handlers wishing to listen for realm connection failure events.
    /// </summary>
    /// <param name="sender">The object that originated the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void RealmFailedEventHandler(object sender, RealmFailedEventArgs e);
}
