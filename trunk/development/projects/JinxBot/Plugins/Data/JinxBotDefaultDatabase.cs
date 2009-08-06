using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    internal class JinxBotDefaultDatabase : IJinxBotDatabase
    {
        private string m_defaultNamespace;

        #region IJinxBotDatabase Members

        public void InitializeConnection(string defaultNamespace, bool isDiablo2)
        {
            m_defaultNamespace = defaultNamespace;
        }

        public IEnumerable<IJinxBotPrincipal> FindUsers(string matchPattern)
        {
            yield break;
        }

        public IJinxBotPrincipal FindExact(global::BNSharp.BattleNet.ChatUser user)
        {
            return new JinxBotDefaultPrincipal(user, m_defaultNamespace);
        }

        public IEnumerable<IJinxBotPrincipal> FindUsersInRole(string role)
        {
            throw new NotImplementedException();
        }

        public void AddUsersToRole(IEnumerable<IJinxBotPrincipal> users, string role)
        {
            
        }

        public void Clear()
        {
            
        }

        public void Save()
        {
            
        }

        #endregion
    }
}
