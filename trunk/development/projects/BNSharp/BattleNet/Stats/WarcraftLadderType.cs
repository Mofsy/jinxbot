using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Stats
{
    [DataContract]
    public enum WarcraftLadderType
    {
        [EnumMember]
        FreeForAll = 0x46464120,
        [EnumMember]
        Solo = 0x534f4c4f,
        [EnumMember]
        Team = 0x5445414d,
    }
}
