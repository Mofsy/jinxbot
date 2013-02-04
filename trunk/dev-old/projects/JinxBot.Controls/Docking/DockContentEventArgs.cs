using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls.Docking
{
#pragma warning disable 1591
    public class DockContentEventArgs : EventArgs
    {
        private IDockContent m_content;

        public DockContentEventArgs(IDockContent content)
        {
            m_content = content;
        }

        public IDockContent Content
        {
            get { return m_content; }
        }
    }
}
#pragma warning restore 1591