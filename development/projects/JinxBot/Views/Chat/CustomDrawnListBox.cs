using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JinxBot.Plugins.UI;
using JinxBot.Views.Chat;
using BNSharp.BattleNet.Clans;
using BNSharp;
using System.Reflection;
using System.Diagnostics;

namespace JinxBot.Views.Chat
{
    public partial class CustomDrawnListBox : ListBox
    {
        private Dictionary<Type, ICustomListBoxItemRenderer> m_renderers;

        public CustomDrawnListBox()
        {
            InitializeComponent();
            base.DrawMode = DrawMode.OwnerDrawVariable;

            m_renderers = new Dictionary<Type, ICustomListBoxItemRenderer>();

            List<CustomListBoxRendererAttribute> customRendererAttributes = new List<CustomListBoxRendererAttribute>();
            Assembly[] allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in allLoadedAssemblies)
            {
                InspectAssemblyForCustomRenderers(customRendererAttributes, asm);
            }
        }

        private void InspectAssemblyForCustomRenderers(List<CustomListBoxRendererAttribute> customRendererAttributes, Assembly asm)
        {
            if (!DesignMode)
            {
                CustomListBoxRendererAttribute[] attributes = asm.GetCustomAttributes(typeof(CustomListBoxRendererAttribute), false) as CustomListBoxRendererAttribute[];
                foreach (CustomListBoxRendererAttribute att in attributes)
                {
                    if (!object.ReferenceEquals(att.RenderedType, null) &&
                        !object.ReferenceEquals(att.RendererType, null) &&
                        !m_renderers.ContainsKey(att.RenderedType))
                    {
                        ICustomListBoxItemRenderer renderer = Activator.CreateInstance(att.RendererType, true) as ICustomListBoxItemRenderer;
                        m_renderers.Add(att.RenderedType, renderer);
                    }
                    else
                    {
                        Debug.WriteLine("A custom list box renderer was applied to the assembly but was configured incorrectly.");
                    }
                }
            }
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
            if (e.Index < 0 || e.Index >= Items.Count)
            {
                base.OnDrawItem(e);
                return;
            }

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
            if (e.Index < 0 || e.Index >= Items.Count)
            {
                base.OnMeasureItem(e);
                return;
            }

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

        private ICustomListBoxItemRenderer GetRenderer(int itemIndex)
        {
            if (!DesignMode)
            {
                object item = Items[itemIndex];
                Type renderedType = item.GetType();
                if (m_renderers.ContainsKey(renderedType))
                {
                    return m_renderers[renderedType];
                }
                else
                {
                    InspectTypeForCustomRenderers(renderedType);
                    return m_renderers[renderedType];
                }
            }
            else
            {
                return null;
            }
        }

        private void InspectTypeForCustomRenderers(Type renderedType)
        {
            if (!DesignMode)
            {
                CustomListBoxRendererAttribute[] attributes = renderedType.GetCustomAttributes(typeof(CustomListBoxRendererAttribute), true) as CustomListBoxRendererAttribute[];
                if (attributes.Length > 0)
                {
                    foreach (CustomListBoxRendererAttribute att in attributes)
                    {
                        if (!object.ReferenceEquals(att.RendererType, null))
                        {
                            ICustomListBoxItemRenderer renderer = Activator.CreateInstance(att.RendererType) as ICustomListBoxItemRenderer;
                            m_renderers.Add(renderedType, renderer);
                            break;
                        }
                        else
                        {
                            Debug.WriteLine("A custom list box renderer was applied to the assembly but was configured incorrectly.");
                        }
                    }
                }
                else
                {
                    m_renderers.Add(renderedType, null);
                }
            }
        }
    }
}
