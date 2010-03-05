using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Contains information about custom measurements required for a custom-drawn list box item.
    /// </summary>
    public sealed class CustomMeasurements 
    {
        private MeasureItemEventArgs m_args;
        private object m_obj;
        /// <summary>
        /// Creates a new <see>CustomMeasurements</see>.
        /// </summary>
        /// <param name="args">The <see>MeasureItemEventArgs</see> contained by this object.</param>
        /// <param name="itemBeingMeasured">The actual item being measured.</param>
        public CustomMeasurements(MeasureItemEventArgs args, object itemBeingMeasured)
        {
            Debug.Assert(!object.ReferenceEquals(args, null));
            Debug.Assert(!object.ReferenceEquals(itemBeingMeasured, null));

            m_args = args;
            m_obj = itemBeingMeasured;
        }

        /// <summary>
        /// Gets a reference to the GDI+ <see>Graphics</see> object used to measure the item.
        /// </summary>
        public Graphics Graphics
        {
            get { return m_args.Graphics; }
        }

        /// <summary>
        /// Gets the index of the item within the list box.
        /// </summary>
        public int Index
        {
            get { return m_args.Index; }
        }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        public int ItemHeight
        {
            get { return m_args.ItemHeight; }
            set { m_args.ItemHeight = value; }
        }

        /// <summary>
        /// Gets or sets the width of the item.
        /// </summary>
        public int ItemWidth
        {
            get { return m_args.ItemWidth; }
            set { m_args.ItemWidth = value; }
        }

        /// <summary>
        /// Gets the item being rendered.
        /// </summary>
        public object Item
        {
            get { return m_obj; }
        }
    }
}
