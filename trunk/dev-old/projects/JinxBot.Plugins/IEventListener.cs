using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp.BattleNet;

namespace JinxBot.Plugins
{
    public interface IEventListener : ISingleClientPlugin
    {
        void HandleClientStartup(BattleNetClient client);

        void HandleClientShutdown(BattleNetClient client);
    }
}
