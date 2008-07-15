using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.McpHandler
{
    public enum RealmConnectionStatus
    {
        Success = 0,
        NoBattleNetConnectionDetected = 0x0c,
        CdKeyBanned = 0x7e,
        TemporaryIpBan = 0x7f,
    }
}
