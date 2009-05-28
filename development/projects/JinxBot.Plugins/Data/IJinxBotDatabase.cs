using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    public interface IJinxBotDatabase
    {
        IEnumerable<IJinxBotPrincipal> FindUsers(string matchPattern);

        IEnumerable<IJinxBotPrincipal> FindUsersInRole(string role);

        void AddUsersToRole(IEnumerable<IJinxBotPrincipal> users, string role);

        void Clear();

        void Save();
    }
}
