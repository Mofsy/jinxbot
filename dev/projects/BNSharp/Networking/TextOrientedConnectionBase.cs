using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    // TODO (for IRC support)
#pragma warning disable 1591
    public class TextOrientedConnectionBase : AsyncConnectionBase
    {
        public TextOrientedConnectionBase(string hostname, int port)
            : base(hostname, port)
        {

        }
    }
#pragma warning restore 1591
}
