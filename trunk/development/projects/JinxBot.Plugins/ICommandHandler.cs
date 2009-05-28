using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;
using System.Security.Principal;

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
        /// <para>The commanding user is represented via the <paramref name="commander"/> parameter.  The use of the <see>IPrincipal</see>
        /// interface allows the command handler to check whether the commander has the authority by using the <see cref="IPrincipal.IsInRole">IsInRole</see>
        /// method.</para>
        /// <example>
        /// <para>For example, assume MyndFyre has issued a !ban brew command:</para>
        /// <code language="csharp">
        /// <![CDATA[
        /// public bool HandleCommand(IPrincipal commander, string command, string[] parameters)
        /// {
        ///     if (command.Equals("ban", StringComparison.OrdinalIgnoreCase)
        ///     {
        ///         if (parameters.Count > 0 && !string.IsNullOrEmpty(parameters[0]))
        ///         {
        ///             if (commander.IsInRole("O"))
        ///             {
        ///                 IPrincipal userToBan = client.Database.FindUser(parameters[0]);
        ///                 if (!userToBan.IsInRole("P")) // protected flag
        ///                 {
        ///                     client.SendMessage(string.Concat("/ban ", parameters[0]));
        ///                     return true;
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     return false;
        /// }
        /// ]]>
        /// </code>
        /// <code language="VB">
        /// <![CDATA[
        /// Public Function HandleCommand(ByVal command As IPrincipal, ByVal command As String, ByVal parameters() As String) As Boolean
        ///     If command.Equals("ban", StringComparison.OrdinalIgnoreCase)
        ///         If parameters.Count > 0 And Not String.IsNullOrEmpty(parameters(0))
        ///             If commander.IsInRole("O")
        ///                 Dim userToBan As IPrincipal = client.Database.FindUser(parameters(0))
        ///                 If Not userToBan.IsInRole("P") ' Protected flag
        ///                     client.SendMessage(String.Concat("/ban ", parameters(0)))
        ///                     Return True
        ///                 End If
        ///             End If
        ///         End If
        ///     End If
        ///     Return False
        /// End Function
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        bool HandleCommand(IPrincipal commander, string command, string[] parameters);

        /// <summary>
        /// Gets an enumerable list of strings that constitute the commands supported by this plugin, and their descriptions.
        /// </summary>
        IEnumerable<string> GetCommandHelp(IPrincipal commander);
    }
}
