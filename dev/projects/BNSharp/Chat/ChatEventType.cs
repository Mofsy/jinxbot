using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Specifies types of chat events that may occur.
    /// </summary>
    public enum ChatEventType
    {
        /// <summary>
        /// Specifies an invalid chat event type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that another user was already in the current channel when it was joined by the client.
        /// </summary>
        UserInChannel = 1,
        /// <summary>
        /// Indicates that another user has joined the current channel.
        /// </summary>
        UserJoinedChannel = 2,
        /// <summary>
        /// Indicates that another user has left the current channel.
        /// </summary>
        UserLeftChannel = 3,
        /// <summary>
        /// Indicates that another user has whispered the client.
        /// </summary>
        WhisperReceived = 4,
        /// <summary>
        /// Indicates that a user in the current channel spoke.
        /// </summary>
        Talk = 5,
        /// <summary>
        /// Indicates that a server operator has broadcast a message.
        /// </summary>
        Broadcast = 6,
        /// <summary>
        /// Indicates that the client joined a new channel.
        /// </summary>
        NewChannelJoined = 7,
        /// <summary>
        /// Indicates that a user's channel properties have been updated.
        /// </summary>
        UserFlagsChanged = 9,
        /// <summary>
        /// Indicates that a whisper was sent.
        /// </summary>
        WhisperSent = 0x0a,
        /// <summary>
        /// Indicates that a channel join failed because the channel is full.
        /// </summary>
        ChannelFull = 0x0d,
        /// <summary>
        /// Indicates that a channel view failed because the channel does not exist.
        /// </summary>
        ChannelDNE = 0x0e,
        /// <summary>
        /// Indicates that a channel join failed because the channel is restricted.
        /// </summary>
        ChannelRestricted = 0x0f,
        /// <summary>
        /// Indicates that the server is relaying information, perhaps from a command.
        /// </summary>
        Information = 0x12,
        /// <summary>
        /// Indicates that there was a command error.
        /// </summary>
        Error = 0x13,
        /// <summary>
        /// Indicates that a user has performed an emote.
        /// </summary>
        Emote = 0x17,
    }
}
