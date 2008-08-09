using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace JinxBot.Plugins.UI
{
    public sealed class CustomMeasurements 
    {
        private MeasureItemEventArgs m_args;
        private object m_obj;
        public CustomMeasurements(MeasureItemEventArgs args, object itemBeingMeasured)
        {
            Debug.Assert(!object.ReferenceEquals(args, null));
            Debug.Assert(!object.ReferenceEquals(itemBeingMeasured, null));

            m_args = args;
            m_obj = itemBeingMeasured;
        }

        public Graphics Graphics
        {
            get { return m_args.Graphics; }
        }

        public int Index
        {
            get { return m_args.Index; }
        }

        public int ItemHeight
        {
            get { return m_args.ItemHeight; }
            set { m_args.ItemHeight = value; }
        }

        public int ItemWidth
        {
            get { return m_args.ItemWidth; }
            set { m_args.ItemWidth = value; }
        }

        public object Item
        {
            get { return m_obj; }
        }
    }
}
