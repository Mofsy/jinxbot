using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    /// <summary>
    /// Represents a connection error.
    /// </summary>
    public class ConnectionErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new ConnectionErrorEventArgs with a corresponding message and exception.
        /// </summary>
        /// <param name="message">The message from the connection error.</param>
        /// <param name="causingException">An exception corresponding to the underlying problem.</param>
        public ConnectionErrorEventArgs(string message, Exception causingException)
        {
            Message = message;
            Exception = causingException;
        }

        /// <summary>
        /// Gets the message from the connection problem.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception corresponding to the underlying problem.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
}
