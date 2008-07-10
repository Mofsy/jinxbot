using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for chat events that involve another user and communication.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ChatMessageEventHandler(object sender, ChatMessageEventArgs e);

    /// <summary>
    /// Represents the event information associated with a chat event with a given user and communication.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ChatMessageEventArgs : ChatEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private UserFlags m_flags;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_un;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_txt;
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ChatMessageEventArgs</see> with the given parameters.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        /// <param name="flags">The other user's flags.</param>
        /// <param name="username">The primarily involved user.</param>
        /// <param name="text">The message.</param>
        public ChatMessageEventArgs(ChatEventType eventType, UserFlags flags, string username, string text)
            : base(eventType)
        {
            m_flags = flags;
            m_un = username;
            m_txt = text;
        }

        /// <summary>
        /// Gets the flags of the user.
        /// </summary>
        public UserFlags Flags { get { return m_flags; } }

        /// <summary>
        /// Gets the name of the user who communicated.
        /// </summary>
        public string Username { get { return m_un; } }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Text { get { return m_txt; } }
    }
}
