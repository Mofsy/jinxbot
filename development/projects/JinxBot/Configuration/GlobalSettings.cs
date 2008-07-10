using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    /// <summary>
    /// Specifies global settings for JinxBot.
    /// </summary>
    [Serializable]
    public class GlobalSettings
    {
        private IconType m_type;

        /// <summary>
        /// Specifies the type of icons to display in the user list.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">Thrown if <paramref name="value" /> is not a valid value defined by the
        /// <see>IconType</see> enumeration.</exception>
        [XmlElement("Icons")]
        public IconType IconType
        {
            get { return m_type; }
            set
            {
                if (!Enum.IsDefined(typeof(IconType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(IconType));
                m_type = value;
            } 
        }

        /// <summary>
        /// Specifies whether to allow data collection.  
        /// </summary>
        [XmlElement("AllowDataCollection")]
        public bool AllowDataCollection
        {
            get;
            set;
        }
    }
}
