using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using JinxBot.Plugins.UI;
using BNSharp.Plugins;

namespace JinxBot.Plugins
{
    internal sealed class PluginInfo
    {
        private Type m_type;
        private string m_name, m_desc, m_author, m_mcName;
        private Version m_ver;

        public PluginInfo(JinxBotPluginAttribute attr, Type t)
        {
            Debug.Assert(t != null);

            m_type = t;

            m_name = attr.Name;
            m_desc = attr.Description;
            m_author = attr.Author;
            m_ver = new Version(attr.Version);
        }

        public PluginInfo(JinxBotPluginAttribute attr, Type t, string multiClientName) 
            : this(attr, t)
        {
            m_mcName = multiClientName;
        }

        public string Name
        {
            get { return m_name; }
        }

        public string MultiClientName
        {
            get { return m_mcName; }
        }

        public string Description
        {
            get { return m_desc; }
        }

        public string Author
        {
            get { return m_author; }
        }

        public Version Version
        {
            get { return m_ver; }
        }

        public Type Type
        {
            get { return m_type; }
        }

        public Assembly Assembly
        {
            get { return m_type.Assembly; }
        }

        public bool SupportsSingleClient
        {
            get { return typeof(ISingleClientPlugin).IsAssignableFrom(m_type); }
        }

        public bool SupportsMultiClient
        {
            get { return typeof(IMultiClientPlugin).IsAssignableFrom(m_type); }
        }

        public bool SupportsCustomPacketHandler
        {
            get { return typeof(IPacketHandlerPlugin).IsAssignableFrom(m_type); }
        }

        public bool SupportsEventListener
        {
            get { return typeof(IEventListener).IsAssignableFrom(m_type); }
        }

        public bool SupportsIconProvider
        {
            get { return typeof(IIconProvider).IsAssignableFrom(m_type); }
        }

        public bool SupportsWardenHandling
        {
            get { return typeof(IWardenModule).IsAssignableFrom(m_type); }
        }
    }
}
