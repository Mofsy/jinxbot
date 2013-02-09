using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
    /// <summary>
    /// Contains information about a situation in which the client failed to log into Battle.net.
    /// </summary>
    public class LoginFailedEventArgs
    {
        private int _statusCode;
        private LoginFailureReason _reason;
        private string _description;
        private bool _supportsExtendedInformation;

        /// <summary>
        /// Creates a new <see>LoginFailedEventArgs</see> that does not support extended information.
        /// </summary>
        /// <param name="reason">The login failure reason associated with this event.</param>
        /// <param name="statusCode">The underlying message status code, which may be useful if the <paramref name="reason"/> parameter is 
        /// <see cref="LoginFailureReason">Unknown</see>.</param>
        public LoginFailedEventArgs(LoginFailureReason reason, int statusCode)
        {
            _reason = reason;
            _statusCode = statusCode;
        }

        /// <summary>
        /// Creates a new <see>LoginFailedEventArgs</see> that does supports extended information.
        /// </summary>
        /// <param name="reason">The login failure reason associated with this event.</param>
        /// <param name="statusCode">The underlying message status code, which may be useful if the <paramref name="reason"/> parameter is 
        /// <see cref="LoginFailureReason">Unknown</see>.</param>
        /// <param name="description">Additional textual information optionally provided by the Battle.net server.</param>
        public LoginFailedEventArgs(LoginFailureReason reason, int statusCode, string description)
            : this(reason, statusCode)
        {
            _description = description;
            _supportsExtendedInformation = true;
        }

        /// <summary>
        /// Gets whether information besides that an invalid username or password was provided.
        /// </summary>
        public bool ProvidesExtendedInformation
        {
            get { return _supportsExtendedInformation; }
        }

        /// <summary>
        /// Gets a textual reason for the login failure, if one was provided by the server.
        /// </summary>
        /// <remarks>
        /// <para>This property is only meaningful if <see>ProvidesExtendedInformation</see> is <see langword="true" />.</para>
        /// </remarks>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the literal status code returned from the server.
        /// </summary>
        public int StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// Gets the basic login failure reason.
        /// </summary>
        public LoginFailureReason Reason
        {
            get { return _reason; }
        }
    }
}
