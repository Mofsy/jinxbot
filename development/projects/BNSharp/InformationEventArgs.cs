using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for event handlers that want to listen to the Information event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void InformationEventHandler(object sender, InformationEventArgs e);

    /// <summary>
    /// Specifies informational event arguments.
    /// </summary>
    [Serializable]
#if !NET_2_ONLY
    [DataContract]
#endif
    public class InformationEventArgs : BaseEventArgs
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_info;
        #endregion

        /// <summary>
        /// Initializes a new <see>InformationEventArgs</see>.
        /// </summary>
        /// <param name="info">The information to pass.</param>
        public InformationEventArgs(string info)
        {
            m_info = info;
        }

        /// <summary>
        /// Gets the information for the event.
        /// </summary>
        public string Information
        {
            get
            {
                return m_info;
            }
        }
    }
}
