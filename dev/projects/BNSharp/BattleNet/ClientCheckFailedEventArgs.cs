using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// Specifies the arguments for a client versioning check failure event.
    /// </summary>
    public class ClientCheckFailedEventArgs
    {
        #region fields
        private ClientCheckFailureCause _reason;
        private string _info;
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ClientCheckFailedEventArgs</see>.
        /// </summary>
        /// <param name="reason">The failure code for version checking.</param>
        /// <param name="additionalInformation">Additional information, if available.</param>
        public ClientCheckFailedEventArgs(ClientCheckFailureCause reason, string additionalInformation)
        {
            _reason = reason;
            _info = additionalInformation;
        }

        /// <summary>
        /// Gets the reason provided by Battle.net.
        /// </summary>
        public ClientCheckFailureCause Reason
        {
            get
            {
                return _reason;
            }
        }

        /// <summary>
        /// Gets additional information, if any, provided by Battle.net about the problem.
        /// </summary>
        public string AdditionalInformation
        {
            get
            {
                return _info;
            }
        }
    }
}
