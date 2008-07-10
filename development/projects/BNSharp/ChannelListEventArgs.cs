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
    public class ChannelListEventArgs : BaseEventArgs
    {
        private string[] m_list;
        internal ChannelListEventArgs(string[] channels)
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

        #region serialization
        private const string CHANNEL_LIST = "ChannelList";

        /// <inheritdoc />
        protected ChannelListEventArgs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_list = info.GetValue(CHANNEL_LIST, typeof(string[])) as string[];
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(CHANNEL_LIST, m_list);
        }
        #endregion
    }
}
