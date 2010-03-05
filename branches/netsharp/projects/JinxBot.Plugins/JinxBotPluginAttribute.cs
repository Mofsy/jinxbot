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

        /// <summary>
        /// Gets the Name of the plugin.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Author of the plugin.
        /// </summary>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Description of the plugin.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Version of the plugin.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Url of the About page, for display in the Plugin Browser feature of JinxBot.
        /// </summary>
        public string Url
        {
            get;
            set;
        }
    }
}
