using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.Data
{
    /// <summary>
    /// When implemented, represents a role.
    /// </summary>
    public interface IJinxBotRole
    {
        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the description of the role.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets an enumeration of strings representing the names of roles that are overridden by this role.
        /// </summary>
        /// <remarks>
        /// <para>Overriding a role name means that, when checking whether a user or his meta have a role, 
        /// if this role overrides that particular role, and the user has this role, that the user won't be 
        /// part of that role.</para>
        /// <example>
        /// <para>Suppose that the meta <c>*ynd*</c> has the role <c>B</c>, and the user <c>MyndFyre</c> has 
        /// the <c>O</c> role.  If the <c>O</c> role overrides <c>B</c>, then even though the <c>B</c>
        /// role might trigger an autoban, the user shouldn't be banned.</para>
        /// </example>
        /// </remarks>
        IEnumerable<string> Overrides { get; }

        /// <summary>
        /// Adds a role to the list of roles overridden by this role.
        /// </summary>
        /// <param name="roleToOverride">The name of the role to override.</param>
        /// <remarks>
        /// <para>Overriding a role name means that, when checking whether a user or his meta have a role, 
        /// if this role overrides that particular role, and the user has this role, that the user won't be 
        /// part of that role.</para>
        /// <example>
        /// <para>Suppose that the meta <c>*ynd*</c> has the role <c>B</c>, and the user <c>MyndFyre</c> has 
        /// the <c>O</c> role.  If the <c>O</c> role overrides <c>B</c>, then even though the <c>B</c>
        /// role might trigger an autoban, the user shouldn't be banned.</para>
        /// </example>
        /// </remarks>
        void AddOverride(string roleToOverride);

        /// <summary>
        /// Removes a role from the list of roles overridden by this role.
        /// </summary>
        /// <param name="roleToNoLongerOverride">The name of the role to no longer override.</param>
        /// <remarks>
        /// <para>Overriding a role name means that, when checking whether a user or his meta have a role, 
        /// if this role overrides that particular role, and the user has this role, that the user won't be 
        /// part of that role.</para>
        /// <example>
        /// <para>Suppose that the meta <c>*ynd*</c> has the role <c>B</c>, and the user <c>MyndFyre</c> has 
        /// the <c>O</c> role.  If the <c>O</c> role overrides <c>B</c>, then even though the <c>B</c>
        /// role might trigger an autoban, the user shouldn't be banned.</para>
        /// </example>
        /// </remarks>
        void RemoveOverride(string roleToNoLongerOverride);
    }
}
