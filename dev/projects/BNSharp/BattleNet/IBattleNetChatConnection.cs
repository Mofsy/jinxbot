using BNSharp.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    public interface IBattleNetChatConnection : IChatConnection
    {
        event EventHandler<ChatMessageEventArgs<UserFlags>> WhisperReceived;
        event EventHandler<ChatMessageEventArgs<UserFlags>> WhisperSent;
    }

    public interface IBattleNetChatConnectionEventSource : IChatConnectionEventSource
    {
        void OnWhisperReceived(ChatMessageEventArgs<UserFlags> args);
        void OnWhisperSent(ChatMessageEventArgs<UserFlags> args);
    }
}
