using BNSharp.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    partial class BattleNetClient 
        : IBattleNetClientEventSource
    {
        public event EventHandler ClientCheckPassed;
        public event EventHandler<ClientCheckFailedEventArgs> ClientCheckFailed;
        public event EventHandler LoginSucceeded;
        public event EventHandler<LoginFailedEventArgs> LoginFailed;

        #region IBattleNetClientEventSource Members
        private void OnClientCheckPassed()
        {
            ((IBattleNetClientEventSource)this).OnClientCheckPassed();
        }

        void IBattleNetClientEventSource.OnClientCheckPassed()
        {
            var temp = ClientCheckPassed;
            if (temp != null) temp(this, EventArgs.Empty);
        }

        private void OnClientCheckFailed(ClientCheckFailedEventArgs args)
        {
            ((IBattleNetClientEventSource)this).OnClientCheckFailed(args);
        }

        void IBattleNetClientEventSource.OnClientCheckFailed(ClientCheckFailedEventArgs args)
        {
            var temp = ClientCheckFailed;
            if (temp != null) temp(this, args);
        }

        private void OnLoginSucceeded()
        {
            ((IChatConnectionEventSource)this).OnLoginSucceeded();
        }

        void IChatConnectionEventSource.OnLoginSucceeded()
        {
            var temp = LoginSucceeded;
            if (temp != null) temp(this, EventArgs.Empty);
        }

        private void OnLoginFailed(LoginFailedEventArgs args)
        {
            ((IChatConnectionEventSource)this).OnLoginFailed(args);
        }

        void IChatConnectionEventSource.OnLoginFailed(LoginFailedEventArgs args)
        {
            var temp = LoginFailed;
            if (temp != null) temp(this, args);
        }

        #endregion

        #region IBattleNetChatConnection Members

        public event EventHandler<ChatMessageEventArgs<UserFlags>> WhisperReceived;

        public event EventHandler<ChatMessageEventArgs<UserFlags>> WhisperSent;

        #endregion

        #region IChatConnection Members


        public event EventHandler<ServerChatEventArgs> Broadcast;

        public event EventHandler<ServerChatEventArgs> ServerError;

        public event EventHandler<ServerChatEventArgs> ServerInformation;

        public event EventHandler<ChannelListEventArgs> ChannelListReceived;

        #endregion

        #region IBattleNetChatConnectionEventSource Members

        void IBattleNetChatConnectionEventSource.OnWhisperReceived(ChatMessageEventArgs<UserFlags> args)
        {
            var tmp = WhisperReceived;
            if (tmp != null)
                tmp(this, args);
        }

        void IBattleNetChatConnectionEventSource.OnWhisperSent(ChatMessageEventArgs<UserFlags> args)
        {
            var tmp = WhisperSent;
            if (tmp != null)
                tmp(this, args);
        }

        #endregion

        #region IChatConnectionEventSource Members


        void IChatConnectionEventSource.OnBroadcast(ServerChatEventArgs args)
        {
            var tmp = Broadcast;
            if (tmp != null)
                tmp(this, args);
        }

        void IChatConnectionEventSource.OnServerError(ServerChatEventArgs args)
        {
            var tmp = ServerError;
            if (tmp != null)
                tmp(this, args);
        }

        void IChatConnectionEventSource.OnServerInformation(ServerChatEventArgs args)
        {
            var tmp = ServerInformation;
            if (tmp != null)
                tmp(this, args);
        }

        void IChatConnectionEventSource.OnChannelListReceived(ChannelListEventArgs args)
        {
            var tmp = ChannelListReceived;
            if (tmp != null)
                tmp(this, args);
        }

        #endregion

        #region IChatConnectionEventSource Members

        private void OnAccountCreated(AccountCreationEventArgs args)
        {
            ((IChatConnectionEventSource)this).OnAccountCreated(args);
        }

        void IChatConnectionEventSource.OnAccountCreated(AccountCreationEventArgs args)
        {
            var tmp = this.AccountCreated;
            if (tmp != null) tmp(this, args);
        }

        private void OnAccountCreationFailed(AccountCreationFailedEventArgs args)
        {
            ((IChatConnectionEventSource)this).OnAccountCreationFailed(args);
        }

        void IChatConnectionEventSource.OnAccountCreationFailed(AccountCreationFailedEventArgs args)
        {
            var tmp = this.AccountCreationFailed;
            if (tmp != null) tmp(this, args);
        }

        private void OnConnected()
        {
            ((IChatConnectionEventSource)this).OnConnected();
        }

        void IChatConnectionEventSource.OnConnected()
        {
            var temp = this.Connected;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            ((IChatConnectionEventSource)this).OnDisconnected();
        }

        void IChatConnectionEventSource.OnDisconnected()
        {
            var temp = this.Disconnected;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        private void OnMessageSent(string message)
        {
            ((IChatConnectionEventSource)this).OnMessageSent(message);
        }

        void IChatConnectionEventSource.OnMessageSent(string message)
        {
            var temp = this.MessageSent;
            if (temp != null)
                temp(this, message);
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ISingleChannelClientEventSource<ChatUser> Members

        void ISingleChannelClientEventSource<ChatUser>.OnPropertyChanged(string propertyName)
        {
            var tmp = PropertyChanged;
            if (tmp != null)
                tmp(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
