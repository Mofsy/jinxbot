using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls
{
    /// <summary>
    /// Contains message information about a message that is ready to be dispatched.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        private string m_message;
        /// <summary>
        /// Creates a new instance of <see>MessageEventArgs</see>.
        /// </summary>
        /// <param name="message">The associated message.</param>
        public MessageEventArgs(string message)
        {
            m_message = message;
        }

        /// <summary>
        /// Gets the message associated with this event.
        /// </summary>
        public string Message
        {
            get { return m_message; }
        }
    }

    /// <summary>
    /// Informs listeners that a message event has been raised.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
}
