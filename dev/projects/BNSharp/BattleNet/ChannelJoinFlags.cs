using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    [Flags]
    internal enum ChannelJoinFlags
    {
        Standard = 0,
        FirstJoin = 1,
        ForcedJoin = 2,
        Diablo2FirstJoin = 5,
    }
}
