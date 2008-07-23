using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.Design;

namespace JinxBot.Configuration
{
    /// <summary>
    /// Specifies the ping style a connection should use.
    /// </summary>
    public enum PingStyle
    {
        /// <summary>
        /// Specifies that the connection should have normal ping.
        /// </summary>
        [Name("Normal")]
        Normal,
        /// <summary>
        /// Specifies that the connection should attempt to have a -1ms ping.
        /// </summary>
        [Name("-1 ms")]
        MinusOneMs,
        /// <summary>
        /// Specifies that the connection should attempt to have a 0ms ping.
        /// </summary>
        [Name("0 ms")]
        ZeroMs,
    }
}
