using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
#pragma warning disable 1591
    // TODO: Document these interfaces

    public interface ISingleChannelClient<TChatUser, TUserFlags>
        : INotifyPropertyChanged
        where TChatUser : IChatUser
        where TUserFlags : struct
    {
        IChannel<TChatUser, TUserFlags> Channel { get; }
        void JoinChannel(string channelName);


    }

    public interface ISingleChannelClientEventSource<TChatUser>
        where TChatUser : IChatUser
    {
        void OnPropertyChanged(string propertyName);
    }
#pragma warning restore 1591
}
