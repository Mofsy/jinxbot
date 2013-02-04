using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace JinxBot.Plugins.Data
{
    /// <summary>
    /// When implemented, identifies the characteristics of a specific user known to the client.
    /// </summary>
    public interface IJinxBotPrincipal 
    {
        /// <summary>
        /// Gets the username, without any Diablo character information or gateway information.
        /// </summary>
        /// <remarks>
        /// <para>This property should return a value such that, if the user originally came into the channel
        /// with the username <c>MyndFyre*MyndFyre[vL]@USEast</c>, the return value is <c>MyndFyre[vL]</c>.</para>
        /// </remarks>
        string Username { get; }

        /// <summary>
        /// Gets the gateway (or namespace) of the user.  
        /// </summary>
        /// <remarks>
        /// <para>When the database is initialized, it is provided with the default gateway of the client (for
        /// instance, on the US East gateway, for Starcraft the default gateway is <c>USEast</c> and for 
        /// Warcraft III clients the default gateway is <c>Azeroth</c>.  This property enables the client to 
        /// distinguish between users on different gateways, since different namespaces provide for the same
        /// username to be replicated but represent different users.</para>
        /// </remarks>
        string Gateway { get; }

        /// <summary>
        /// Gets an enumerable list of roles to which the user belongs.
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// Checks whether the user belongs to a specific role.
        /// </summary>
        /// <param name="roleName">The name of the role to check.</param>
        /// <returns><see langword="true" /> if the current user is in the specified role; otherwise
        /// <see langword="false" />.</returns>
        bool IsInRole(string roleName);
    }
}
