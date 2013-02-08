using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Core
{
    /// <summary>
    /// Designates a callback method that can be used to parse a specific packet.  This delegate is not CLS-compliant.
    /// </summary>
    /// <param name="packetData">The contents of the packet.</param>
    /// <remarks>
    /// <para>This delegate should only be used by advanced developers when registering custom packet handlers with BN#.</para>
    /// </remarks>
    [CLSCompliant(false)]
    public delegate void ParseCallback(BncsReader packetData);
}
