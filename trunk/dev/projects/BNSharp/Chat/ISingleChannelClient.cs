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

    public interface ISingleChannelClient<TChatUser>
        : INotifyPropertyChanged
        where TChatUser : IChatUser
    {
        IChannel<TChatUser> Channel { get; }
        void JoinChannel(string channelName);


    }

    public interface ISingleChannelClientEventSource<TChatUser>
        where TChatUser : IChatUser
    {
        void OnPropertyChanged(string propertyName);
    }
#pragma warning restore 1591
}
