using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace JinxBot.Plugins.Data
{
    internal class JinxBotDefaultPrincipal : IJinxBotPrincipal
    {
        private JinxBotDefaultIdentity m_id;

        internal JinxBotDefaultPrincipal(JinxBotDefaultIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            m_id = identity;
        }

        #region IJinxBotPrincipal Members

        public IJinxBotIdentity Identity
        {
            get { return m_id; }
        }

        IIdentity IPrincipal.Identity
        {
            get
            {
                return this.Identity;
            }
        }

        #endregion

        #region IPrincipal Members

        public bool IsInRole(string role)
        {
            return false;
        }

        #endregion
    }
}
