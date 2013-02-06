using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Represents a channel of chat users.
    /// </summary>
    /// <typeparam name="TChatUser">The type of chat user.  This type must inherit from the IChatUser interface.</typeparam>
    public interface IChannel<TChatUser> : INotifyPropertyChanged
        where TChatUser : IChatUser
    {
        /// <summary>
        /// Gets the name of the current channel.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets an enumeration of chat users currently in the channel.
        /// </summary>
        IEnumerable<TChatUser> Users
        {
            get;
        }
    }
}
