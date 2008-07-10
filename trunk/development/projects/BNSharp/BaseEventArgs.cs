using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace BNSharp
{
    /// <summary>
    /// Provides the base class from which all BN# event argument class should derive.
    /// </summary>
    [Serializable]
    public class BaseEventArgs : EventArgs //, ISerializable, IXmlSerializable
    {
        /// <summary>
        /// Creates a new instance of <see>BaseEventArgs</see>.
        /// </summary>
        protected BaseEventArgs() { }

        [NonSerialized]
        private object m_parse;
        /// <summary>
        /// Gets or sets the underlying connection data that was used to drive this event.
        /// </summary>
        internal object EventData
        {
            get { return m_parse; }
            set { m_parse = value; }
        }

        /// <summary>
        /// Gets a new empty BaseEventArgs object for a specified event data object.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        internal static BaseEventArgs GetEmpty(object eventData)
        {
            BaseEventArgs e = new BaseEventArgs();
            e.EventData = eventData;
            return e;
        }

        /// <summary>
        /// Gets the name of this EventArgs type.
        /// </summary>
        public string TypeName
        {
            get { return GetType().Name; }
        }

        #region ISerializable Members
        private const string SER_TYPENAME = "TypeName";

        /// <summary>
        /// Initializes a <see>BaseEventArgs</see> from a serialization stream.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected BaseEventArgs(SerializationInfo info, StreamingContext context)
        {

        }

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        #endregion

        #region IXmlSerializable Members

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual void ReadXml(XmlReader reader)
        {

        }

        /// <inheritdoc />
        public virtual void WriteXml(XmlWriter writer)
        {
            
        }

        /// <summary>
        /// Writes an element and value to an XML writer during serialization.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <param name="content">The value to write.</param>
        protected virtual void WriteValue(XmlWriter writer, string elementName, string content)
        {
            writer.WriteAttributeString(elementName, content);
        }

        #endregion
    }
}
