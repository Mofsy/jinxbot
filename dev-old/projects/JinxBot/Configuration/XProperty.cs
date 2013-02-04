using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace JinxBot.Configuration
{
    public class XProperty : IXmlLineInfo
    {
        private XAttribute m_attr;
        private XElement m_elem;

        public XProperty() { }

        public XProperty(XAttribute attribute)
        {
            m_attr = attribute;
        }

        public XProperty(XElement element)
        {
            m_elem = element;
        }

        public string Value
        {
            get
            {
                if (m_attr != null) return m_attr.Value;
                if (m_elem != null) return m_elem.Value;
                return null;
            }
        }

        public XElement AsElement
        {
            get { return m_elem; }
        }

        #region IXmlLineInfo Members

        bool IXmlLineInfo.HasLineInfo()
        {
            if (m_attr != null)
            {
                return (m_attr as IXmlLineInfo).HasLineInfo();
            }
            else if (m_elem != null)
            {
                return (m_elem as IXmlLineInfo).HasLineInfo();
            }
            else
            {
                return false;
            }
        }

        int IXmlLineInfo.LineNumber
        {
            get 
            {
                if (m_attr != null)
                {
                    return (m_attr as IXmlLineInfo).LineNumber;
                }
                else if (m_elem != null)
                {
                    return (m_elem as IXmlLineInfo).LineNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        int IXmlLineInfo.LinePosition
        {
            get
            {
                if (m_attr != null)
                {
                    return (m_attr as IXmlLineInfo).LinePosition;
                }
                else if (m_elem != null)
                {
                    return (m_elem as IXmlLineInfo).LinePosition;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion
    }
}
