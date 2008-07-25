using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;
using BNSharp.BattleNet.Friends;
using System.Drawing;
using System.Windows.Forms;

namespace JinxBot.Views.Chat
{
    public class FriendListBoxItemRenderer : ICustomListBoxItemRenderer
    {
        private IIconProvider m_provider;
        internal FriendListBoxItemRenderer()
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
            if ((e.State & DrawItemState.Selected) == System.Windows.Forms.DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }
            FriendUser friend = e.Item as FriendUser;

            Color textColor = e.ForeColor;
            if (friend.LocationType == FriendLocation.Offline)
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
                e.Graphics.DrawImage(m_provider.GetImageFor(friend.Product), (PointF)iconPosition);

                SizeF nameSize = e.Graphics.MeasureString(friend.AccountName, e.Font);
                RectangleF nameArea = new RectangleF((float)e.Bounds.X + (float)m_provider.IconSize.Width + 1.0f + 4.0f,
                    (float)e.Bounds.Y + (((float)e.Bounds.Height - nameSize.Height) / 2.0f),
                    (float)e.Bounds.Width - (float)m_provider.IconSize.Width - 2.0f - 4.0f,
                    (float)nameSize.Height);
                e.Graphics.DrawString(friend.AccountName, e.Font, textBrush, nameArea, nameFormat);
            }
        }

        #endregion
    }
}
