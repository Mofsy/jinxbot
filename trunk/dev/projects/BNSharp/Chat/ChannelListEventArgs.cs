using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Specifies the channel list event arguments.
    /// </summary>
    [DebuggerDisplay("{_list.Length} channel(s).")]
    public class ChannelListEventArgs
    {
        #region fields
        private string[] _list;
        #endregion
        /// <summary>
        /// Creates a new instance of <see>ChannelListEventArgs</see>.
        /// </summary>
        /// <param name="channels">The channels to list.</param>
        public ChannelListEventArgs(string[] channels)
        {
            _list = channels;
        }

        /// <summary>
        /// Gets the copy of the list of channels sent by the server.
        /// </summary>
        public ReadOnlyCollection<string> Channels
        {
            get
            {
                return new ReadOnlyCollection<string>(_list);
            }
        }
    }
}
