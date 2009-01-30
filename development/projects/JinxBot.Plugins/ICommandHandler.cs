using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins
{
    /// <summary>
    /// Specifies the interface required for a custom command handler.
    /// </summary>
    public interface ICommandHandler : ISingleClientPlugin
    {
        bool HandleCommand(string command, string[] parameters);
    }
}
