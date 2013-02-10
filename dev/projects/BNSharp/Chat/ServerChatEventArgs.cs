using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Specifies event information for chat events that do not involve another user.
    /// </summary>
    /// <para>An example of when this class would be used is for a server broadcast, server information, or an error.</para>
    public class ServerChatEventArgs : ChatEventArgs
    {
        #region fields
        private int _flags;
        private string _txt;
        #endregion

        /// <summary>
        /// Creates a new <see>ServerChatEventArgs</see> with the specified information.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        /// <param name="flags">Event-specific flags that must be interpreted based on the event type.</param>
        /// <param name="text">Informational message from the server.</param>
        public ServerChatEventArgs(ChatEventType eventType, int flags, string text)
            : base(eventType)
        {
            _flags = flags;
            _txt = text;
        }

        /// <summary>
        /// Gets the message from the server.
        /// </summary>
        public string Text
        {
            get
            {
                return _txt;
            }
        }

        /// <summary>
        /// Gets the event-specific flags.
        /// </summary>
        /// <remarks>
        /// <para>These must be interpreted based on the event type.</para>
        /// </remarks>
        /// <seealso cref="UserFlags"/>
        /// <seealso cref="ChannelFlags"/>
        public int Flags
        {
            get { return _flags; }
        }
    }
}
