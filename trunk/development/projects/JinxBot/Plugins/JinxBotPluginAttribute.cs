using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins
{
    /// <summary>
    /// When applied to a class, indicates that the type should be treated as a plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class JinxBotPluginAttribute : Attribute
    {
        public JinxBotPluginAttribute() { }

        public string Name
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }
    }
}
