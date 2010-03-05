using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins;
using JinxBot.Plugins.UI;
using BNSharp.BattleNet.Clans;
using System.Drawing;
using System.Windows.Forms;

namespace JinxBot.Views.Chat
{
    internal sealed class ClanListBoxItemRenderer : ICustomListBoxItemRenderer
    {
        private IIconProvider m_provider;
        public ClanListBoxItemRenderer()
        {
            ProfileResourceProvider prp = ProfileResourceProvider.GetForClient(null);
            if (!object.ReferenceEquals(prp, null))
                m_provider = prp.Icons;
        }

        #region ICustomListBoxItemRenderer Members

        public void MeasureItem(CustomMeasurements e)
        {
            e.ItemHeight = m_provider.IconSize.Height + 2;
        }

        public void DrawItem(CustomItemDrawData e)
        {
            e.DrawBackground();
            if ((e.State & DrawItemState.Focus) == System.Windows.Forms.DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }

            ClanMember clanMember = e.Item as ClanMember;
            Color textColor = e.ForeColor;
            if (clanMember.CurrentStatus == ClanMemberStatus.Offline)
            {
                if ((e.State & DrawItemState.Selected) == System.Windows.Forms.DrawItemState.Selected)
                {
                    textColor = Color.Black;
                }
                else
                {
                    textColor = Color.SlateGray;
                }
            }
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (StringFormat nameFormat = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter })
            {
                PointF iconPosition = new PointF((float)e.Bounds.Location.X + 1.0f, (float)e.Bounds.Location.Y + 1.0f);
                e.Graphics.DrawImage(m_provider.GetImageFor(clanMember.Rank), (PointF)iconPosition);

                SizeF nameSize = e.Graphics.MeasureString(clanMember.Username, e.Font);
                RectangleF nameArea = new RectangleF((float)e.Bounds.X + (float)m_provider.IconSize.Width + 1.0f + 4.0f,
                    (float)e.Bounds.Y + (((float)e.Bounds.Height - nameSize.Height) / 2.0f),
                    (float)e.Bounds.Width - (float)m_provider.IconSize.Width - 2.0f - 4.0f,
                    (float)nameSize.Height);
                e.Graphics.DrawString(clanMember.Username, e.Font, textBrush, nameArea, nameFormat);
            }
        }

        #endregion
    }
}
