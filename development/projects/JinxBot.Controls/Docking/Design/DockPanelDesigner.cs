using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace JinxBot.Controls.Docking.Design
{
    internal class DockPanelDesigner : ParentControlDesigner
    {
        private DesignerVerbCollection m_verbs;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (m_verbs == null)
                {
                    m_verbs = new DesignerVerbCollection();
                    m_verbs.AddRange(base.Verbs);

                    m_verbs.Add(new DesignerVerb(DesignResources.AddPanelLeft, new EventHandler(AddPanelToLeft_Clicked)));
                    m_verbs.Add(new DesignerVerb(DesignResources.AddPanelBottom, new EventHandler(AddPanelToBottom_Clicked)));
                    m_verbs.Add(new DesignerVerb(DesignResources.AddPanelRight, new EventHandler(AddPanelToRight_Clicked)));
                }

                return m_verbs;
            }
        }

        private void AddPanelToLeft_Clicked(object sender, EventArgs e)
        {
            AddDefaultContentToTarget(DockAreas.DockLeft);
        }

        private void AddDefaultContentToTarget(DockAreas target)
        {
            DockPanel panel = this.Control as DockPanel;
            if (panel == null)
                return;

            DockContent content = new DockContent();
            content.DockAreas = target;
            panel.AddContent(content);
            content.Show(panel);
            content.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockBottom;
        }

        private void AddPanelToRight_Clicked(object sender, EventArgs e)
        {
            AddDefaultContentToTarget(DockAreas.DockRight);
        }

        private void AddPanelToBottom_Clicked(object sender, EventArgs e)
        {
            AddDefaultContentToTarget(DockAreas.DockBottom);
        }
    }
}
