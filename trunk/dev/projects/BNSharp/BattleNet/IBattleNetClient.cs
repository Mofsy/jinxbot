using BNSharp.BattleNet.Warden;
using BNSharp.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    public interface IBattleNetClient : 
        IChatConnection,
        ISingleChannelClient<ChatUser, UserFlags>
    {
        IWardenModule WardenHandler { get; set; }

        event EventHandler ClientCheckPassed;
        event EventHandler<ClientCheckFailedEventArgs> ClientCheckFailed;
    }

    public interface IBattleNetClientEventSource
    {
        void OnClientCheckPassed();
        void OnClientCheckFailed(ClientCheckFailedEventArgs args);
    }
}
