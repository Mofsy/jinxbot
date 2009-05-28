using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.Data
{
    public interface IJinxBotIdentity : IIdentity
    {
        ChatUser BattleNetUser { get; }

        string DefaultUsername { get; }
    }
}
