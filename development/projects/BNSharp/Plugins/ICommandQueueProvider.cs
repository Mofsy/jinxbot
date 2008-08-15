using System;

namespace BNSharp.Plugins
{
    /// <summary>
    /// The <see>ICommandQueueProvider</see> interface is not yet supported in BN# Beta 2.
    /// </summary>
    [Obsolete("ICommandQueueProvider is not available in BN# Beta 2.")]
    public interface ICommandQueueProvider
    {
        /// <summary>
        /// Determines whether sending a message of the specified length will cause a flood condition on the server.
        /// </summary>
        /// <param name="messageLength">The length of the message being tested.</param>
        /// <returns><see langword="true"/> if sending the message will cause a flood; otherwise <see langword="false" />.</returns>
        bool WillMessageFlood(int messageLength);
        /// <summary>
        /// Calculates the message delay and allows the command queue to perform any additional calculations for the message.
        /// </summary>
        /// <param name="messageLength">The length of the message being sent.</param>
        /// <returns>A value specifying the number of milliseconds to delay after sending the message.</returns>
        long SetupDelay(int messageLength);
    }
}
