using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    /// <summary>
    /// When implemented, represents a meta-collection of users.
    /// </summary>
    /// <remarks>
    /// <para>Metas are a way to assign behaviors to groups of users who fall within a specific match pattern.
    /// For instance, consider the meta <c>"*re*"</c> with a role of B.  If a user named <c>brew</c>
    /// entered the channel, that user would automatically be assigned the B role, and plugins would act 
    /// accordingly (for example, a channel moderation plugin might automatically ban the user).</para>
    /// <para>For more information about how metas should affect users, see 
    /// <see cref="M:IJinxBotDatabase.FindExact">the FindExact method</see> of <see>IJinxBotDatabase</see>.</para>
    /// </remarks>
    public interface IJinxBotMeta
    {
        /// <summary>
        /// Gets the match pattern to check.
        /// </summary>
        string MatchPattern { get; }

        /// <summary>
        /// Gets an enumerable list of roles.
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// Checks whether the specified meta is in a role.
        /// </summary>
        /// <param name="role">The name of the role to check.</param>
        /// <returns><see langword="true" /> if the meta is in the role; otherwise <see langword="false" />.</returns>
        bool IsInRole(string role);
    }
}
