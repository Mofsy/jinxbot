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
        ISingleChannelClient<ChatUser>
    {
        IWardenModule WardenHandler { get; set; }
    }
}
