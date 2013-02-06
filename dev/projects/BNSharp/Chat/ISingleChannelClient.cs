using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    public interface ISingleChannelClient<TChatUser>
        : INotifyPropertyChanged
        where TChatUser : IChatUser
    {
        IChannel<TChatUser> Channel { get; }
        void JoinChannel(string channelName);


    }
}
