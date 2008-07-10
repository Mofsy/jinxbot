using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for clients wishing to register for the channel list event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ChannelListEventHandler(object sender, ChannelListEventArgs e);

    /// <summary>
    /// Specifies the channel list event arguments.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class ChannelListEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private string[] m_list;
        #endregion
        /// <summary>
        /// Creates a new instance of <see>ChannelListEventArgs</see>.
        /// </summary>
        /// <param name="channels">The channels to list.</param>
        public ChannelListEventArgs(string[] channels)
        {
            m_list = channels;
        }

        /// <summary>
        /// Gets the copy of the list of channels sent by the server.
        /// </summary>
        public string[] Channels
        {
            get
            {
                string[] channels = new string[m_list.Length];
                Array.Copy(m_list, channels, channels.Length);
                return channels;
            }
        }
    }
}
