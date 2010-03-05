using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    /// <summary>
    /// When implemented, modifies the behavior of <see cref="IJinxBotMeta.IsInRole(string)">IJinxBotMeta.IsInRole</see> and 
    /// <see cref="IJinxBotPrincipal.IsInRole(string)">IJinxBotUser.IsInRole</see> by allowing for additional checks for permission.
    /// </summary>
    /// <remarks>
    /// <para>Sometimes, a role can imply another role; however, because of the complexity in allowing roles to both override and imply each other, rather, 
    /// the JinxBot system intrinsically only supports overriding.  A role translator plugin, then, allows JinxBot plugins to opt-in to extending the database system
    /// without additional work on the part of the database provider.</para>
    /// <para>Database plugin developers should implement the appropriate IRoleTranslator-related methods in IJinxBotDatabase, and utilize translators within the calls of
    /// <see cref="IJinxBotMeta.IsInRole(string)">IJinxBotMeta.IsInRole</see> and <see cref="IJinxBotPrincipal.IsInRole(string)">IJinxBotUser.IsInRole</see>.  The appropriate
    /// line of checking should be the normal implementation first, then return true immediately if any implemented translators return true.</para>
    /// <example>
    /// <para>For instance, the StealthBot moderation plugin uses a role translator in order to implement a StealthBot-compatible Access Level system utilizing the intrinsic
    /// roles functionality.  In this plugin, the StealthBot moderation plugin adds only the highest access level to which a user belongs to the database, and then the translator
    /// detects the highest access level and returns <see langword="true" /> for any lower access levels.</para>
    /// </example>
    /// </remarks>
    public interface IRoleTranslator
    {
        /// <summary>
        /// Checks whether <paramref name="roleSought"/> is part of the <paramref name="rolesOwned"/> collection, given the translator's rules.
        /// </summary>
        /// <param name="rolesOwned">The database-provided roles.</param>
        /// <param name="roleSought">The role to check.</param>
        /// <returns><see langword="true" /> if the translator's rules indicate that the user should own the role; otherwise <see langword="false" />.</returns>
        bool IsInRole(IEnumerable<string> rolesOwned, string roleSought);
    }
}
