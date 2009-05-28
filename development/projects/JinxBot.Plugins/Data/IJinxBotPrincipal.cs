using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace JinxBot.Plugins.Data
{
    public interface IJinxBotPrincipal : IPrincipal
    {
        IJinxBotIdentity Identity { get; }
    }
}
