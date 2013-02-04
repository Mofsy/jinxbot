using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.McpHandler
{
    public class RealmServer
    {
        private string m_title;
        private string m_desc;

        public RealmServer(string title, string description)
        {
            m_title = title;
            m_desc = description;
        }

        public string Title
        {
            get { return m_title; }
        }

        public string Description
        {
            get
            {
                return m_desc;
            }
        }
    }
}
