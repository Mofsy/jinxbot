using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;

namespace JinxBot.Plugins
{
    /// <summary>
    /// Specifies the interface required for a custom command handler.
    /// </summary>
    public interface ICommandHandler : IJinxBotPlugin
    {
        /// <summary>
        /// Instructs a command handler to attempt to handle a command.
        /// </summary>
        /// <param name="commander">The user instructing the client to perform the action.</param>
        /// <param name="command">The command text.</param>
        /// <param name="parameters">Parameters following the command.</param>
        /// <returns><see langword="true" /> if the plugin handles the command; otherwise <see langword="false" />.</returns>
        /// <remarks>
        /// <para>A plugin that handles command should decorate itself as an <see>ICommandHandler</see> so that JinxBot knows to prompt it to parse
        /// commands (JinxBot itself implements support for command triggering).  When a command is triggered, JinxBot enumerates through 
        /// command-handling plugins.  Once a plugin returns <see langword="true" />, the enumeration terminates.  Consequently, only one 
        /// command-handling plugin is able to handle the command.</para>
        /// <para>Well-behaved JinxBot command handling plugins should not handle the command and then return <see langword="false" />.  However,
        /// to allow multiple commands of the same name, the plugin should verify that the argument list is what is expected before handling and 
        /// returning <see langword="true" />.</para>
        /// </remarks>
        bool HandleCommand(ChatUser commander, string command, string[] parameters);

        /// <summary>
        /// Gets an enumerable list of strings that constitute the commands supported by this plugin, and their descriptions.
        /// </summary>
        IEnumerable<string> CommandHelp { get; }
    }
}
