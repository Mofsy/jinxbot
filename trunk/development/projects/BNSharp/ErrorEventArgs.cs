using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for event handlers wishing to listen to the error event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    /// <summary>
    /// Specifies error event information.
    /// </summary>
    [DataContract]
    public class ErrorEventArgs : BaseEventArgs
    {
        #region fields
        [DataMember(Name = "Error")]
        private string m_err;
        [DataMember(Name = "IsDisconnecting")]
        private bool m_disc; 
        #endregion

        /// <summary>
        /// Creates a new instance of <see>ErrorEventArgs</see>.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="disconnecting">Whether it is causing the client to disconnect.</param>
        public ErrorEventArgs(string error, bool disconnecting)
        {
            m_err = error;
            m_disc = disconnecting;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Error
        {
            get { return m_err; }
        }

        /// <summary>
        /// Whether it is causing the client to disconnect.
        /// </summary>
        public bool IsDisconnecting
        {
            get { return m_disc; }
        }
    }
}
