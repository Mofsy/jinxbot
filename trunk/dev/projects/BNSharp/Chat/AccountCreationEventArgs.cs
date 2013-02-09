using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Contains information about an attempted account creation event.
    /// </summary>
    public class AccountCreationEventArgs
    {
        private string _acctName;

        /// <summary>
        /// Creates a new <see>AccountCreationEventArgs</see> for the specified account name.
        /// </summary>
        /// <param name="accountName">The name of the account being created.</param>
        public AccountCreationEventArgs(string accountName)
        {
            Debug.Assert(!string.IsNullOrEmpty(accountName));
            _acctName = accountName;
        }

        /// <summary>
        /// Gets the name of the account being created.
        /// </summary>
        public string AccountName
        {
            get { return _acctName; }
        }
    }
}
