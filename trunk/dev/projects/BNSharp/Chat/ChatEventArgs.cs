using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Provides the base information about a chat event.
    /// </summary>
    [DebuggerDisplay("Type={EventType}")]
    public abstract class ChatEventArgs
    {
        private ChatEventType m_evType;
        /// <summary>
        /// Initializes a new <see>ChatEventArgs</see>.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        protected ChatEventArgs(ChatEventType eventType)
        {
            m_evType = eventType;
        }

        /// <summary>
        /// Gets the type of chat event that took place.
        /// </summary>
        public ChatEventType EventType
        {
            get { return m_evType; }
        }
    }
}
