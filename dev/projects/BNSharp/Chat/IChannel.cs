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
    public interface IChannel<TChatUser, TUserFlags> : INotifyPropertyChanged
        where TChatUser : IChatUser 
        where TUserFlags : struct
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

        event EventHandler<UserEventArgs<TChatUser>> UserShown;
        event EventHandler<UserEventArgs<TChatUser>> UserJoined;
        event EventHandler<UserEventArgs<TChatUser>> UserLeft;

        event EventHandler<ChatMessageEventArgs<TUserFlags>> UserSpoke;
        event EventHandler<ChatMessageEventArgs<TUserFlags>> UserEmoted;
        event EventHandler<UserEventArgs<TChatUser>> UserFlagsChanged; 
    }

    public interface IChannelEventSource<TChatUser, TUserFlags>
        where TChatUser : IChatUser
        where TUserFlags : struct
    {
        void OnUserShown(UserEventArgs<TChatUser> args);
        void OnUserJoined(UserEventArgs<TChatUser> args);
        void OnUserLeft(UserEventArgs<TChatUser> args);

        void OnUserSpoke(ChatMessageEventArgs<TUserFlags> args);
        void OnUserEmoted(ChatMessageEventArgs<TUserFlags> args);
        void OnUserFlagsChanged(UserEventArgs<TChatUser> args);
    }
}
