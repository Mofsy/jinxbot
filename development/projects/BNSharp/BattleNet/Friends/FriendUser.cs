using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Friends
{
    /// <summary>
    /// Represents a Battle.net friend user.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class FriendUser
    {
    }
}
