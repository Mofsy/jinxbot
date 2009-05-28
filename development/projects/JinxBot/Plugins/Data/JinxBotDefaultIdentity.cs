using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.Data
{
    internal class JinxBotDefaultIdentity : IJinxBotIdentity
    {
        private ChatUser m_user;

        #region IJinxBotIdentity Members

        public ChatUser BattleNetUser
        {
            get { return m_user; }
        }

        public string DefaultUsername
        {
            get { return m_user.Username; }
        }

        #endregion

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return "Battle.net"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return m_user.Username; }
        }

        #endregion
    }
}
