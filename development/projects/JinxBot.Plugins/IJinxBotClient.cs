using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;
using JinxBot.Plugins.UI;
using JinxBot.Plugins.Data;

namespace JinxBot.Plugins
{
    public interface IJinxBotClient
    {
        BattleNetClient Client { get; }
        IChatTab MainWindow { get; }
        IJinxBotDatabase Database { get; }

        void SendMessage(string message);
    }
}
