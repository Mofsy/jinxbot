using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Provides the base information about a chat event.
    /// </summary>
    public abstract class ChatEventArgs : BaseEventArgs
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

        #region serialization
        private const string SER_EVENT_TYPE = "EventType";

        /// <inheritdoc />
        protected ChatEventArgs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_evType = (ChatEventType)info.GetInt32(SER_EVENT_TYPE);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(SER_EVENT_TYPE, (int)m_evType);
        }
        #endregion
    }
}
