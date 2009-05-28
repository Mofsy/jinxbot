using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    internal class JinxBotDefaultDatabase : IJinxBotDatabase
    {
        #region IJinxBotDatabase Members

        public IEnumerable<IJinxBotPrincipal> FindUsers(string matchPattern)
        {
            return new IJinxBotPrincipal[0];
        }

        public IEnumerable<IJinxBotPrincipal> FindUsersInRole(string role)
        {
            return new IJinxBotPrincipal[0];
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
