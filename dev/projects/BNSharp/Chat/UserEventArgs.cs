using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Specifies event information for chat events that involve another user, but not specifically communication.
    /// </summary>
    /// <para>An example of when this class would be used is for a user joined or user left event.</para>
    public class UserEventArgs<TChatUser> : ChatEventArgs
        where TChatUser : IChatUser
    {
        #region fields
        private TChatUser _user;
        #endregion

        /// <summary>
        /// Creates a new <see>UserEventArgs</see> with the specified settings.
        /// </summary>
        /// <param name="eventType">The type of chat event.</param>
        /// <param name="user">A reference to the user involved in the event.</param>
        public UserEventArgs(ChatEventType eventType, TChatUser user)
            : base(eventType)
        {
            Debug.Assert(user != null);

            _user = user;
        }

        /// <summary>
        /// Gets a reference to the user who was involved in the event.
        /// </summary>
        public TChatUser User
        {
            get { return _user; }
        }
    }
}
