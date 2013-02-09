using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Contains information about an account creation attempt that failed.
    /// </summary>
    public class AccountCreationFailedEventArgs : AccountCreationEventArgs
    {
        private CreationFailureReason _reason;

        /// <summary>
        /// Creates a new <see>AccountCreationFailedEventArgs</see> for the specifiec account.
        /// </summary>
        /// <param name="accountName">The name that failed to be created.</param>
        /// <param name="reason">The reason provided by Battle.net for the failure.</param>
        public AccountCreationFailedEventArgs(string accountName, CreationFailureReason reason)
            : base(accountName)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason for the account creation failure.
        /// </summary>
        public CreationFailureReason Reason
        {
            get { return _reason; }
        }
    }
}
