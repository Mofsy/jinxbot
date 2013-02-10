using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Represents the event information associated with a chat event with a given user and communication.
    /// </summary>
    [DebuggerDisplay("Type={EventType}, User={Username}, \"{Text}\"")]
    public class ChatMessageEventArgs<TFlagsType> : ChatEventArgs
        where TFlagsType : struct
    {
        #region fields
        private TFlagsType _flags;
        private string _un;
        private string _txt;
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ChatMessageEventArgs</see> with the given parameters.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        /// <param name="flags">The other user's flags.</param>
        /// <param name="username">The primarily involved user.</param>
        /// <param name="text">The message.</param>
        public ChatMessageEventArgs(ChatEventType eventType, TFlagsType flags, string username, string text)
            : base(eventType)
        {
            _flags = flags;
            _un = username;
            _txt = text;
        }

        /// <summary>
        /// Gets the flags of the user.
        /// </summary>
        public TFlagsType Flags { get { return _flags; } }

        /// <summary>
        /// Gets the name of the user who communicated.
        /// </summary>
        public string Username { get { return _un; } }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Text { get { return _txt; } }
    }
}
