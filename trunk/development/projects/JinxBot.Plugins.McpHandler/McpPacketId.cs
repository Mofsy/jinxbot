using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.McpHandler
{
    public enum McpPacketId
    {
        Startup = 1,
        CharacterCreate = 2,
        CreateGame = 3,
        JoinGame =4 ,
        GameList = 5,
        GameInfo = 6,
        CharacterLogon = 7,
        CharacterDelete = 0x0a,
        RequestLadderData = 0x11,
        MessageOfTheDay = 0x12,
        CancelGameCreate = 0x13,
        CreateQueue = 0x14,
        CharacterList = 0x17,
        CharacterUpgrade = 0x18,
        CharacterList2 = 0x19,
    }
}
