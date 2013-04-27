using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Core
{
    internal static class InternalExtensions
    {
        public static string ToHexString(this BigInteger bigInt)
        {
            return bigInt.ToString("x");
        }
    }
}
