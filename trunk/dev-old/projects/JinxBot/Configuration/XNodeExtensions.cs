using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JinxBot.Configuration
{
    internal static class XNodeExtensions
    {
        public static XProperty Property(this XElement parentNode, XName name)
        {
            XElement element = parentNode.Element(name);
            if (element != null)
                return new XProperty(element);

            XAttribute attr = parentNode.Attribute(name);
            if (attr != null)
                return new XProperty(attr);

            return new XProperty();
        }

        public static IEnumerable<XProperty> Properties(this XElement parentNode)
        {
            if (parentNode == null)
                yield break;

            foreach (XAttribute at in parentNode.Attributes())
                yield return new XProperty(at);

            foreach (XElement el in parentNode.Elements())
                yield return new XProperty(el);
        }

        public static IEnumerable<XProperty> Properties(this XElement parentNode, XName name)
        {
            if (parentNode == null)
                yield break;

            foreach (XAttribute at in parentNode.Attributes(name))
                yield return new XProperty(at);

            foreach (XElement el in parentNode.Elements(name))
                yield return new XProperty(el);
        }

        public static bool AsBool(this XElement node)
        {
            bool result = false;
            if (node != null)
            {
                bool.TryParse(node.Value, out result);
            }
            return result;
        }

        public static bool AsBool(this XProperty node)
        {
            bool result = false;
            if (node != null)
            {
                bool.TryParse(node.Value, out result);
            }
            return result;
        }

        public static bool AsBool(this XAttribute node)
        {
            bool result = false;
            if (node != null)
            {
                bool.TryParse(node.Value, out result);
            }
            return result;
        }

        public static int AsInt32(this XElement node)
        {
            int result = 0;
            if (node != null)
            {
                int.TryParse(node.Value, out result);
            }
            return result;
        }

        public static int AsInt32(this XAttribute node)
        {
            int result = 0;
            if (node != null)
            {
                int.TryParse(node.Value, out result);
            }
            return result;
        }

        public static int AsInt32(this XProperty node)
        {
            int result = 0;
            if (node != null)
            {
                int.TryParse(node.Value, out result);
            }
            return result;
        }

        public static T As<T>(this XElement parent, Func<XElement, T> parser)
        {
            if (parent == null)
                return default(T);
            return parser(parent);
        }

        public static IEnumerable<T> As<T>(this IEnumerable<XElement> parents, Func<XElement, T> parser)
        {
            foreach (XElement parent in parents)
            {
                if (parent == null) continue;
                yield return parent.As<T>(parser);
            }
        }
    }
}
