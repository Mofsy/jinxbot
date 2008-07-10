using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp
{
    /// <summary>
    /// Specifies the contract for event handlers wishing to listen to the entered chat event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void EnteredChatEventHandler(object sender, EnteredChatEventArgs e);

    /// <summary>
    /// Specifies the event arguments for when the client entered chat.
    /// </summary>
    public class EnteredChatEventArgs : BaseEventArgs
    {
        private string m_uun, m_ss, m_an;
        internal EnteredChatEventArgs(string uniqueName, string statstring, string acctName)
        {
            m_uun = uniqueName;
            m_ss = statstring;
            m_an = acctName;
        }

        /// <summary>
        /// Gets the unique username assigned to the client.
        /// </summary>
        public string UniqueUsername { get { return m_uun; } }

        /// <summary>
        /// Gets the user's client information string.
        /// </summary>
        public string Statstring { get { return m_ss; } }

        /// <summary>
        /// Gets the user's login account name.
        /// </summary>
        public string AccountName { get { return m_an; } }

        #region serialization
        private const string SER_UNIQUE = "UniqueName";
        private const string SER_STATS = "Statstring";
        private const string SER_NAME = "AccountName";

        /// <inheritdoc />
        protected EnteredChatEventArgs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_uun = info.GetString(SER_UNIQUE);
            m_ss = info.GetString(SER_STATS);
            m_an = info.GetString(SER_NAME);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(SER_UNIQUE, m_uun);
            info.AddValue(SER_STATS, m_ss);
            info.AddValue(SER_NAME, m_an);
        }

        /// <summary>
        /// Creates a default instance of <see>EnteredChatEventArgs</see>.
        /// </summary>
        /// <remarks>
        /// <para>This constructor supports XML serialization and is not intended to be used from your code.</para>
        /// </remarks>
        public EnteredChatEventArgs() { }

        /// <inheritdoc />
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);

            m_uun = reader.GetAttribute(SER_UNIQUE);
            m_ss = reader.GetAttribute(SER_STATS);
            m_an = reader.GetAttribute(SER_NAME);
        }

        /// <inheritdoc />
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

            WriteValue(writer, SER_UNIQUE, m_uun);
            WriteValue(writer, SER_STATS, m_ss);
            WriteValue(writer, SER_STATS, m_an);
        }
        #endregion
    }
}
