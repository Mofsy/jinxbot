using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.Data
{
    /// <summary>
    /// When implemented in a class, realizes the functionality for a basic JinxBot database.
    /// </summary>
    /// <remarks>
    /// <para>This interface is by no means exhaustive, and plugin developers interested in building a plugin
    /// to accomodate channel operations or similar functionality should feel free to implement this database
    /// and share implementation details between this database interface and their operations plugin.  The 
    /// ideal situation would be to not do so, but it does not seem to represent a violation of encapsulation.</para>
    /// <para>Most JinxBot databases will define some concept of "roles."  For example, the person who owns 
    /// the client will probably be granted the "Owner" or "O" role.  JinxBot does not define roles, but 
    /// provides the infrastructure to allow them, because most command-based plugins will likely require 
    /// them.</para>
    /// <para>For command-based plugins that want to use rank levels or some type of numeric system, the 
    /// usual <see cref="M:IJinxBotPrincipal.IsInRole">IsInRole</see> method will not be adequate without some
    /// additional work on the part of the command interpreter.  For instance, if a user is granted a numeric
    /// level of 200 for access, and a new command is added that requires access level 125, the user would 
    /// require that the role "125" be added to his list of roles.</para>
    /// <para>By inheriting from <see>IJinxBotPlugin</see>, a database should load and save its settings, and
    /// possibly its data (if necessary) via the <see cref="M:IJinxBotPlugin.Startup">Startup</see> and 
    /// <see cref="M:IJinxBotPlugin.Shutdown">Shutdown</see> methods.</para>
    /// </remarks>
    public interface IJinxBotDatabase : IJinxBotPlugin
    {
        /// <summary>
        /// Initializes the database with connection-specific parameters.
        /// </summary>
        /// <param name="defaultNamespace">This should be the namespace of the client, such as 
        /// USEast, Azeroth, or others.  See remarks for more information.</param>
        /// <param name="isDiablo2">Specifies whether the client is a Diablo II client with specific realm
        /// character prefixes.</param>
        /// <remarks>
        /// <para>The namespace should always be included and determined by the name or server of the 
        /// currently-connected gateway.  This way, the default namespace of the user can be assigned for 
        /// storage, so if the connection is changed to a different namespace, user associations can remain.
        /// For instance, consider the situation in which a database must be shared across six operations
        /// bots holding a channel (one Starcraft + five Warcraft III, for instance).  If the channel is in 
        /// the US East gateway, the Starcraft client should initialize its database with the default 
        /// namespace of <c>"USEast"</c> and the Warcraft III clients should initialize it to <c>"Azeroth"</c>.  
        /// Then, when a user named "Biff" joins the channel with Diablo II, the Starcraft client can see that 
        /// user as "Biff", "USEast", and the Warcraft III clients (which see Biff@USEast) can also distinguish
        /// appropriately.</para>
        /// </remarks>
        void InitializeConnection(string defaultNamespace, bool isDiablo2);

        /// <summary>
        /// Retrieves a list of all users who match the specified pattern.
        /// </summary>
        /// <param name="matchPattern">The requested pattern to match.</param>
        /// <returns>An enumerable list of users who meet the requested pattern.</returns>
        /// <remarks>
        /// <para>The "match pattern" is specifically left vague to allow database developers to implement 
        /// their own matching patterns internally.  However, after the Beta 1 release this may change to 
        /// be a sanitized regular expression of some kind.</para>
        /// </remarks>
        IEnumerable<IJinxBotPrincipal> FindUsers(string matchPattern);

        /// <summary>
        /// Finds a user account for the specified Battle.net user.
        /// </summary>
        /// <param name="user">The user to find.</param>
        /// <returns>An object implementing <see>IJinxBotPrincipal</see> representing the rights of the specified
        /// user.</returns>
        /// <remarks>
        /// <para>This method should never return <see langword="null" />.  While it is acceptable to not to
        /// try to guess when searching for users in the <see>FindUsers</see> method, because a concrete
        /// Battle.net user is being passed into the database, this method should return a concrete identity
        /// back to the client.</para>
        /// </remarks>
        IJinxBotPrincipal FindExact(ChatUser user);

        /// <summary>
        /// Retrieves a list of all users who are assigned to a specified role.
        /// </summary>
        /// <param name="role">The name of the role to search.</param>
        /// <returns>An enumerable list of users who are in the specified role.</returns>
        /// <seealso cref="59c4110e-6f5b-4c1d-bbd0-027c906d751b"/>
        IEnumerable<IJinxBotPrincipal> FindUsersInRole(string role);

        /// <summary>
        /// Adds a role to a specific user.
        /// </summary>
        /// <param name="user">The user to modify.</param>
        /// <param name="role">The name of the role to add.</param>
        /// <seealso cref="59c4110e-6f5b-4c1d-bbd0-027c906d751b"/>
        void AddUserToRole(IJinxBotPrincipal user, string role);

        /// <summary>
        /// Removes a role from a specific user.
        /// </summary>
        /// <param name="user">The user to modify.</param>
        /// <param name="role">The name of the role to remove.</param>
        /// <seealso cref="59c4110e-6f5b-4c1d-bbd0-027c906d751b"/>
        void RemoveRoleFromUser(IJinxBotPrincipal user, string role);

        void AddRoleToMeta(string matchPattern, string role);

        void RemoveRoleFromMeta(string matchPattern, string role);



        /// <summary>
        /// Clears all users from the database.
        /// </summary>
        /// <remarks>
        /// <para>This method should be used with caution.</para>
        /// </remarks>
        void Clear();
    }
}
