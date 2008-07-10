using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Provides the base information about a chat event.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public abstract class ChatEventArgs : BaseEventArgs
    {
        [DataMember]
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
