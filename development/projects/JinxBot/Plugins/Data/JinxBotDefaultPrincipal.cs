using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using BNSharp.BattleNet;
using System.Text.RegularExpressions;

namespace JinxBot.Plugins.Data
{
    internal class JinxBotDefaultPrincipal : IJinxBotPrincipal
    {
        private const string UsernameParser = @"\A(?<charName>[^*@\s]*?)?\*?(?<accountName>[^*@\s]+)(?:@(?<gateway>\w+))?\z";
        private const string CHARNAME = "charName";
        private const string ACCTNAME = "accountName";
        private const string GATEWAY = "gateway";

        private static Regex Parser = new Regex(UsernameParser, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string m_userName, m_gateway, m_charName;

        public JinxBotDefaultPrincipal(ChatUser user, string defaultGateway)
        {
            Match m = Parser.Match(user.Username);
            if (!m.Success)
                throw new ArgumentException("The specified user had an invalid username.");

            m_userName = m.Groups[ACCTNAME].Value;
            if (m.Groups[GATEWAY].Success)
                m_gateway = m.Groups[GATEWAY];
            else
                m_gateway = defaultGateway;

            if (m.Groups[CHARNAME].Success)
                m_charName = m.Groups[CHARNAME];
        }

        #region IJinxBotPrincipal Members

        public string Username
        {
            get { return m_userName; }
        }

        public string Gateway
        {
            get { return m_gateway; }
        }

        public IEnumerable<string> Roles
        {
            get { yield break; }
        }

        public bool IsInRole(string roleName)
        {
            return false;
        }

        #endregion
    }
}
