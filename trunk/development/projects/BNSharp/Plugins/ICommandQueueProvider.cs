using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.Plugins
{
    public interface ICommandQueueProvider
    {
        bool WillMessageFlood(int messageLength);
        /// <summary>
        /// Calculates the message delay and allows the command queue 
        /// </summary>
        /// <param name="messageLength"></param>
        /// <returns></returns>
        long SetupDelay(int messageLength);
    }
}
