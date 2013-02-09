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
    }
}
