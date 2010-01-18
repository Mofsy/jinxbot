using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Contains information about a custom-drawn object for a list box.
    /// </summary>
    public class CustomItemDrawData : DrawItemEventArgs
    {
        private object m_item;

        /// <summary>
        /// Creates a new instance of the <see>CustomItemDrawData</see> object.
        /// </summary>
        /// <param name="args">The <see>DrawItemEventArgs</see> on which this instance is based.</param>
        /// <param name="itemToDraw">The object to render.</param>
        public CustomItemDrawData(DrawItemEventArgs args, object itemToDraw)
            : base(args.Graphics, args.Font, args.Bounds, args.Index, args.State, args.ForeColor, args.BackColor)
        {
            Debug.Assert(!object.ReferenceEquals(null, itemToDraw));

            m_item = itemToDraw;
        }

        /// <summary>
        /// Gets the object to be drawn by the custom drawing renderer.
        /// </summary>
        public object Item
        {
            get { return m_item; }
        }
    }
}
