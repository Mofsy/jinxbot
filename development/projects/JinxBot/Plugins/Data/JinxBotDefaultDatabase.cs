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
            yield break;
        }

        public void AddUserToRole(IJinxBotPrincipal user, string role)
        {
            
        }

        public void RemoveRoleFromUser(IJinxBotPrincipal user, string role)
        {
            
        }

        public void AddRoleToMeta(string matchPattern, string role)
        {
            
        }

        public void RemoveRoleFromMeta(string matchPattern, string role)
        {
            
        }

        public void Clear()
        {
            
        }

        public IEnumerable<IJinxBotRole> DefinedRoles
        {
            get { yield break; }
        }

        #endregion

        #region IJinxBotPlugin Members

        public void Startup(IDictionary<string, string> settings)
        {
            
        }

        public void Shutdown(IDictionary<string, string> settings)
        {
            
        }

        public object GetSettingsObject()
        {
            return new object();
        }

        public IPluginUpdateManifest CheckForUpdates()
        {
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
