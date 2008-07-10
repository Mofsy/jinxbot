using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.Plugins
{
    /// <summary>
    /// When implemented, allows a custom class to handle Warden packets.
    /// </summary>
    public interface IWardenModule
    {
        /// <summary>
        /// Initializes the Warden module with the specified CD key hash part, the native socket handle, and the game file.
        /// </summary>
        /// <param name="keyHashPart">The key hash part provided.</param>
        /// <param name="socketHandle">The native OS socket handle to optionally directly send results.</param>
        /// <param name="gameFile3">The path to the Battle.snp file.</param>
        /// <returns>Nonzero if it succeeds; otherwise zero.</returns>
        int InitWarden(int keyHashPart, IntPtr socketHandle, string gameFile3);
        /// <summary>
        /// Uninitializes the Warden module.
        /// </summary>
        void UninitWarden();
        /// <summary>
        /// Processes the warden challenge.
        /// </summary>
        /// <param name="wardenPacket">A copy of the original buffer sent by the server.</param>
        void ProcessWarden(byte[] wardenPacket);
    }
}
