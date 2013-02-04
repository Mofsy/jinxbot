using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace JinxBot.Plugins.JinxBotWeb
{
    [DataContract]
    public enum ClientEventType
    {
        [EnumMember]
        ChannelDidNotExist,
        [EnumMember]
        ChannelListReceived,
        [EnumMember]
        ChannelFull,
        [EnumMember]
        ChannelRestricted,
        [EnumMember]
        ClientCheckFailed,
        [EnumMember]
        ClientCheckPassed,
        [EnumMember]
        CommandSent,
        [EnumMember]
        Connected,
        [EnumMember]
        Disconnected,
        [EnumMember]
        EnteredChat,
        [EnumMember]
        Error,
        [EnumMember]
        Information,
        [EnumMember]
        InformationReceived,
        [EnumMember]
        JoinedChannel,
        [EnumMember]
        LoginFailed,
        [EnumMember]
        LoginSucceeded,
        [EnumMember]
        MessageSent,
        [EnumMember]
        ServerBroadcast,
        [EnumMember]
        ServerError,
        [EnumMember]
        UserEmoted,
        [EnumMember]
        UserFlags,
        [EnumMember]
        UserJoined,
        [EnumMember]
        UserLeft,
        [EnumMember]
        UserShown,
        [EnumMember]
        UserSpoke,
        [EnumMember]
        WardenUnhandled,
        [EnumMember]
        WhisperReceived,
        [EnumMember]
        WhisperSent,
    }
}
