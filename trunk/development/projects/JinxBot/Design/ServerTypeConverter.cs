using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace JinxBot.Design
{
    internal class ServerTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null || value.GetType() != typeof(string))
            {
                string fullDescription = value as string;
                if (fullDescription.IndexOf(':') <= 0) // -1 is if : is not in the string.  0 if there is no host name
                {
                    throw new FormatException("The specified value was invalid.  A server must be entered in the format \"host-name:port\".");
                }
                else
                {
                    string[] hostAndPort = fullDescription.Split(':');
                    return new Server(hostAndPort[0], int.Parse(hostAndPort[1]));
                }
            }
            return null;
        }
    }
}
