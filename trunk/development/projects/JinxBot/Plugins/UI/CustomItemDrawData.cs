using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace JinxBot.Plugins.UI
{
    public class CustomItemDrawData : DrawItemEventArgs
    {
        private object m_item;

        internal CustomItemDrawData(DrawItemEventArgs args, object itemToDraw)
            : base(args.Graphics, args.Font, args.Bounds, args.Index, args.State, args.ForeColor, args.BackColor)
        {
            Debug.Assert(!object.ReferenceEquals(null, itemToDraw));

            m_item = itemToDraw;
        }

        public object Item
        {
            get { return m_item; }
        }
    }
}
