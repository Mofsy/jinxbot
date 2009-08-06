using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.BattleNet;
using BNSharp.BattleNet.Clans;
using BNSharp.BattleNet.Friends;
using BNSharp.BattleNet.Stats;

namespace BNSharp.Plugins
{
    public interface IBattleNetEventSource
    {
        event AccountCreationEventHandler AccountCreated;
        event AccountCreationFailedEventHandler AccountCreationFailed;
        event AdChangedEventHandler AdChanged;
        event ServerChatEventHandler ChannelDidNotExist;
        event ChannelListEventHandler ChannelListReceived;
        event ServerChatEventHandler ChannelWasFull;
        event ServerChatEventHandler ChannelWasRestricted;
        event ClanCandidatesSearchEventHandler ClanCandidatesSearchCompleted;
        event ClanChieftanChangeEventHandler ClanChangeChieftanCompleted;
        event ClanDisbandEventHandler ClanDisbandCompleted;
        event ClanFormationEventHandler ClanFormationCompleted;
        event ClanFormationInvitationEventHandler ClanFormationInvitationReceived;
        event ClanInvitationEventHandler ClanInvitationReceived;
        event ClanInvitationResponseEventHandler ClanInvitationResponseReceived;
        event ClanMemberListEventHandler ClanMemberListReceived;
        event ClanMemberStatusEventHandler ClanMemberQuit;
        event ClanMemberRankChangeEventHandler ClanMemberRankChanged;
        event ClanMemberStatusEventHandler ClanMemberRemoved;
        event ClanMembershipEventHandler ClanMembershipReceived;
        event ClanMemberStatusEventHandler ClanMemberStatusChanged;
        event InformationEventHandler ClanMessageOfTheDay;
        event ClanRankChangeEventHandler ClanRankChangeResponseReceived;
        event ClanRemovalResponseEventHandler ClanRemovalResponse;
        event ClientCheckFailedEventHandler ClientCheckFailed;
        event EventHandler ClientCheckPassed;
        event InformationEventHandler CommandSent;
        event EventHandler Connected;
        event EventHandler Disconnected;
        event EnteredChatEventHandler EnteredChat;
        event ErrorEventHandler Error;
        event FriendAddedEventHandler FriendAdded;
        event FriendListReceivedEventHandler FriendListReceived;
        event FriendMovedEventHandler FriendMoved;
        event FriendRemovedEventHandler FriendRemoved;
        event FriendUpdatedEventHandler FriendUpdated;
        event InformationEventHandler Information;
        event ServerChatEventHandler InformationReceived;
        event ServerChatEventHandler JoinedChannel;
        event LeftClanEventHandler LeftClan;
        event LoginFailedEventHandler LoginFailed;
        event EventHandler LoginSucceeded;
        event ChatMessageEventHandler MessageSent;
        event ProfileLookupFailedEventHandler ProfileLookupFailed;
        event ServerChatEventHandler ServerBroadcast;
        event ServerChatEventHandler ServerErrorReceived;
        event ServerNewsEventHandler ServerNews;
        event ChatMessageEventHandler UserEmoted;
        event UserEventHandler UserFlagsChanged;
        event UserEventHandler UserJoined;
        event UserEventHandler UserLeft;
        event UserProfileEventHandler UserProfileReceived;
        event UserEventHandler UserShown;
        event ChatMessageEventHandler UserSpoke;
        event WarcraftProfileEventHandler WarcraftProfileReceived;
        event EventHandler WardenUnhandled;
        event ChatMessageEventHandler WhisperReceived;
        event ChatMessageEventHandler WhisperSent;
    }
}
