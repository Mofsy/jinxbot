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
    public class ChatMessageEventArgs : ChatEventArgs
    {
        private UserFlags m_flags;
        private string m_un, m_txt;

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

        #region serialization

        private const string SER_USERNAME = "UserName";
        private const string SER_TEXT = "Text";
        private const string SER_FLAGS = "Flags";

        /// <inheritdoc />
        protected ChatMessageEventArgs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue(SER_USERNAME, m_un);
            info.AddValue(SER_TEXT, m_txt);
            info.AddValue(SER_FLAGS, (int)m_flags);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            m_un = info.GetString(SER_USERNAME);
            m_txt = info.GetString(SER_TEXT);
            m_flags = (UserFlags)info.GetInt32(SER_FLAGS);
        }
        #endregion
    }
}
