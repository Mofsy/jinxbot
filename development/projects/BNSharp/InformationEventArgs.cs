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
    public class InformationEventArgs : BaseEventArgs
    {
        private string m_info;

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

        #region serialization support
        private const string SER_INFO = "info";

        /// <inheritdoc />
        protected InformationEventArgs(SerializationInfo info, StreamingContext context)
        {
            m_info = info.GetString(SER_INFO);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(SER_INFO, m_info);
        }

        /// <inheritdoc />
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
        }

        /// <inheritdoc />
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }
        #endregion
    }
}
