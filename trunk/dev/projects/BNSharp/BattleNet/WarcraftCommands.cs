using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    internal enum WarcraftCommands : byte
    {
        RequestLadderMap = 2,
        CancelLadderGameSearch = 3,
        UserInfoRequest = 4,
        ClanInfoRequest = 8,
        IconListRequest = 9,
        ChangeIcon = 10,
    }
}
