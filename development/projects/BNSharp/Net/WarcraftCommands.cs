using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.Net
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
