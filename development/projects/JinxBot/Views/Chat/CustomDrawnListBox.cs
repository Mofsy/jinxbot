using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Plugins.UI;

namespace JinxBot.Views.Chat
{
    public partial class CustomDrawnListBox : ListBox
    {
        public CustomDrawnListBox()
        {
            InitializeComponent();
            base.DrawMode = DrawMode.OwnerDrawVariable;
        }

        public override DrawMode DrawMode
        {
            get
            {
                return DrawMode.OwnerDrawVariable;
            }
            set { }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            ICustomListBoxItemRenderer renderer = GetRenderer(e.Index);
            if (object.ReferenceEquals(null, renderer))
            {
                base.OnDrawItem(e);
            }
            else
            {
                CustomItemDrawData draw = new CustomItemDrawData(e, Items[e.Index]);
                renderer.DrawItem(draw);
            }
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            ICustomListBoxItemRenderer renderer = GetRenderer(e.Index);
            if (object.ReferenceEquals(null, renderer))
            {
                base.OnMeasureItem(e);
            }
            else
            {
                CustomMeasurements measurements = new CustomMeasurements(e, Items[e.Index]);
                e.ItemWidth = Width;
                renderer.MeasureItem(measurements);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        private ICustomListBoxItemRenderer m_renderer = new ChannelListBoxItemRenderer();
        private ICustomListBoxItemRenderer GetRenderer(int itemIndex)
        {
            return m_renderer;
        }
    }
}
