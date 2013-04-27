using BNSharp.BattleNet.Core;
using BNSharp.BattleNet.Stats;
using BNSharp.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    internal class BattleNetClientChannel : IChannel<ChatUser, UserFlags>, IChannelEventSource<ChatUser, UserFlags>
    {
        private string _channelName;
        private Dictionary<string, ChatUser> _namesToUsers;
        private BattleNetClient _client;
        private ParseCallback _oldParseCallback;

        public BattleNetClientChannel(BattleNetClient source)
        {
            _namesToUsers = new Dictionary<string, ChatUser>();
            _client = source;
            _oldParseCallback = _client.RegisterParseCallback(BncsPacketId.ChatEvent, HandleChatEvent);
        }


        #region chat events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void HandleChatEvent(BncsReader dr)
        {
            if (_oldParseCallback != null)
            {
                _oldParseCallback(dr);
                dr.Seek(-dr.Position);
                dr.Seek(4);
            }

            ChatEventType type = (ChatEventType)dr.ReadInt32();
            int flags = dr.ReadInt32();
            int ping = dr.ReadInt32();
            dr.Seek(12);
            string user = dr.ReadCString();
            byte[] userInfo = dr.ReadNullTerminatedByteArray();
            string text = Encoding.ASCII.GetString(userInfo);

            switch (type)
            {
                case ChatEventType.UserInChannel:
                case ChatEventType.UserJoinedChannel:
                    ChatUser newUser = new ChatUser(user, ping, (ClassicUserFlags)flags, UserStats.Parse(user, userInfo));
                    if (_namesToUsers.ContainsKey(user))
                    {
                        _namesToUsers.Remove(user);
                    }
                    _namesToUsers.Add(user, newUser);
                    UserEventArgs<ChatUser> uArgs = new UserEventArgs<ChatUser>(type, newUser);
                    HandleUserChatEvent(uArgs);
                    break;
                case ChatEventType.UserFlagsChanged:
                    if (_namesToUsers.ContainsKey(user))
                    {
                        ChatUser changedUser = _namesToUsers[user];
                        changedUser.Flags = (ClassicUserFlags)flags;
                        UserEventArgs<ChatUser> updatedArgs = new UserEventArgs<ChatUser>(type, changedUser);
                        HandleUserChatEvent(updatedArgs);
                    }
                    else if (_channelName.Equals("The Void", StringComparison.OrdinalIgnoreCase))
                    {
                        ChatUser voidUser = new ChatUser(user, ping, (ClassicUserFlags)flags, UserStats.Parse(user, userInfo));
                        _namesToUsers.Add(user, voidUser);
                        UserEventArgs<ChatUser> voidArgs = new UserEventArgs<ChatUser>(type, voidUser);
                        HandleUserChatEvent(voidArgs);
                    }
                    break;
                case ChatEventType.UserLeftChannel:
                    if (_namesToUsers.ContainsKey(user))
                    {
                        ChatUser goneUser = _namesToUsers[user];
                        UserEventArgs<ChatUser> leftArgs = new UserEventArgs<ChatUser>(type, goneUser);
                        HandleUserChatEvent(leftArgs);
                    }
                    break;
                case ChatEventType.Emote:
                case ChatEventType.Talk:
                case ChatEventType.WhisperReceived:
                case ChatEventType.WhisperSent:
                    ChatMessageEventArgs<UserFlags> cmArgs = new ChatMessageEventArgs<UserFlags>(type, (UserFlags)flags, user, Encoding.UTF8.GetString(userInfo));
                    HandleChatMessageEvent(cmArgs);
                    break;
                case ChatEventType.NewChannelJoined:
                    ServerChatEventArgs joinArgs = new ServerChatEventArgs(type, flags, text);
                    _channelName = text;
                    _namesToUsers.Clear();
                    OnJoinedChannel(joinArgs);
                    break;
                case ChatEventType.Broadcast:
                case ChatEventType.ChannelDNE:
                case ChatEventType.ChannelFull:
                case ChatEventType.ChannelRestricted:
                case ChatEventType.Error:
                case ChatEventType.Information:
                    ServerChatEventArgs scArgs = new ServerChatEventArgs(type, flags, text);
                    HandleServerChatEvent(scArgs);
                    break;
            }
        }

        private void HandleServerChatEvent(ServerChatEventArgs scArgs)
        {
            switch (scArgs.EventType)
            {
                case ChatEventType.Broadcast:
                    ((IChatConnectionEventSource)_client).OnBroadcast(scArgs);
                    break;
                case ChatEventType.ChannelDNE:
                case ChatEventType.ChannelFull:
                case ChatEventType.ChannelRestricted:
                case ChatEventType.Error:
                    ((IChatConnectionEventSource)_client).OnServerError(scArgs);
                    break;
                case ChatEventType.Information:
                    ((IChatConnectionEventSource)_client).OnServerInformation(scArgs);
                    break;
            }
        }

        internal void HandleChatMessageEvent(ChatMessageEventArgs<UserFlags> cmArgs)
        {
            switch (cmArgs.EventType)
            {
                case ChatEventType.Emote:
                    OnUserEmoted(cmArgs);
                    break;
                case ChatEventType.Talk:
                    OnUserSpoke(cmArgs);
                    break;
                case ChatEventType.WhisperReceived:
                    ((IBattleNetChatConnectionEventSource)_client).OnWhisperReceived(cmArgs);
                    break;
                case ChatEventType.WhisperSent:
                    ((IBattleNetChatConnectionEventSource)_client).OnWhisperSent(cmArgs);
                    break;
            }
        }

        private void HandleUserChatEvent(UserEventArgs<ChatUser> userEventArgs)
        {
            switch (userEventArgs.EventType)
            {
                case ChatEventType.UserFlagsChanged:
                    OnUserFlagsChanged(userEventArgs);
                    break;
                case ChatEventType.UserInChannel:
                    OnUserShown(userEventArgs);
                    break;
                case ChatEventType.UserJoinedChannel:
                    OnUserJoined(userEventArgs);
                    break;
                case ChatEventType.UserLeftChannel:
                    OnUserLeft(userEventArgs);
                    break;
            }
        }
        #endregion

        private void OnJoinedChannel(ServerChatEventArgs args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnNewChannelJoined(args);
        }

        #region IChannel<ChatUser,UserFlags> Members

        public string Name
        {
            get { return _channelName; }
        }

        public IEnumerable<ChatUser> Users
        {
            get { return _namesToUsers.Values.AsEnumerable(); }
        }

        public event EventHandler<ServerChatEventArgs> NewChannelJoined;

        public event EventHandler<UserEventArgs<ChatUser>> UserShown;

        public event EventHandler<UserEventArgs<ChatUser>> UserJoined;

        public event EventHandler<UserEventArgs<ChatUser>> UserLeft;

        public event EventHandler<ChatMessageEventArgs<UserFlags>> UserSpoke;

        public event EventHandler<ChatMessageEventArgs<UserFlags>> UserEmoted;

        public event EventHandler<UserEventArgs<ChatUser>> UserFlagsChanged;

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IChannelEventSource<ChatUser,UserFlags> Members

        void IChannelEventSource<ChatUser, UserFlags>.OnNewChannelJoined(ServerChatEventArgs args)
        {
            var tmp = NewChannelJoined;
            if (tmp != null)
                tmp(this, args);
        }

        private void OnUserShown(UserEventArgs<ChatUser> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserShown(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserShown(UserEventArgs<ChatUser> args)
        {
            var tmp = this.UserShown;
            if (tmp != null) tmp(this, args);
        }

        private void OnUserJoined(UserEventArgs<ChatUser> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserJoined(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserJoined(UserEventArgs<ChatUser> args)
        {
            var tmp = this.UserJoined;
            if (tmp != null) tmp(this, args);
        }

        private void OnUserLeft(UserEventArgs<ChatUser> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserLeft(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserLeft(UserEventArgs<ChatUser> args)
        {
            var tmp = this.UserLeft;
            if (tmp != null) tmp(this, args);
        }

        private void OnUserSpoke(ChatMessageEventArgs<UserFlags> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserSpoke(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserSpoke(ChatMessageEventArgs<UserFlags> args)
        {
            var tmp = this.UserSpoke;
            if (tmp != null) tmp(this, args);
        }

        private void OnUserEmoted(ChatMessageEventArgs<UserFlags> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserEmoted(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserEmoted(ChatMessageEventArgs<UserFlags> args)
        {
            var tmp = this.UserEmoted;
            if (tmp != null) tmp(this, args);
        }

        private void OnUserFlagsChanged(UserEventArgs<ChatUser> args)
        {
            ((IChannelEventSource<ChatUser, UserFlags>)this).OnUserFlagsChanged(args);
        }

        void IChannelEventSource<ChatUser, UserFlags>.OnUserFlagsChanged(UserEventArgs<ChatUser> args)
        {
            var tmp = this.UserFlagsChanged;
            if (tmp != null) tmp(this, args);
        }

        #endregion
    }
}
