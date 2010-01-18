using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.BattleNet;
using BNSharp.BattleNet.Clans;
using BNSharp.BattleNet.Friends;
using BNSharp.BattleNet.Stats;

namespace BNSharp.Plugins
{
#pragma warning disable 1591
    public interface IBattleNetEventSource
    {
        /// <summary>
        /// Informs listeners that a new account has been created.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterAccountCreatedNotification</see> and
        /// <see>UnregisterAccountCreatedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        event AccountCreationEventHandler AccountCreated;
        /// <summary>
        /// Registers for notification of the <see>AccountCreated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="AccountCreated" />
        /// <seealso cref="UnregisterAccountCreatedNotification" />
        void RegisterAccountCreatedNotification(Priority p, AccountCreationEventHandler callback);
        /// <summary>
        /// Unregisters for notification of the <see>AccountCreated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="AccountCreated" />
        /// <seealso cref="RegisterAccountCreatedNotification" />
        void UnregisterAccountCreatedNotification(Priority p, AccountCreationEventHandler callback);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
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
#pragma warning restore 1591
}
