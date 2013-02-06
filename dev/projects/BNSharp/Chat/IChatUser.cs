using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Represents a generic chat user.
    /// </summary>
    public interface IChatUser
    {
        /// <summary>
        /// Gets the name of the user as seen by the client.
        /// </summary>
        string Username { get; }
    }
}
