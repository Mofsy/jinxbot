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

        private ClanListBoxItemRenderer m_renderer = new ClanListBoxItemRenderer();
        private ICustomListBoxItemRenderer GetRenderer(int itemIndex)
        {
            return m_renderer;
        }
    }
}
