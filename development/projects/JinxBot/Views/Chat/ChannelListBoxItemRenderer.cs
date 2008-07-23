using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;
using BNSharp;
using System.Drawing;
using BNSharp.BattleNet.Stats;
using System.Globalization;

namespace JinxBot.Views.Chat
{
    public class ChannelListBoxItemRenderer : ICustomListBoxItemRenderer
    {
        private IIconProvider m_provider;
        internal ChannelListBoxItemRenderer()
        {
            m_provider = ProfileResourceProvider.GetForClient(null).Icons;
        }

        #region ICustomListBoxItemRenderer Members

        public void MeasureItem(CustomMeasurements e)
        {
            e.ItemHeight = m_provider.IconSize.Height + 2;
        }

        public void DrawItem(CustomItemDrawData e)
        {
            e.DrawBackground();
            if (e.State == System.Windows.Forms.DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }

            UserEventArgs user = e.Item as UserEventArgs;
            using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
            using (StringFormat nameFormat = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter })
            {
                UserStats stats = UserStats.Parse(user.Username, user.StatsData);
                PointF iconPosition = new PointF((float)e.Bounds.Left + 1.0f, (float)e.Bounds.Top + 1.0f);
                e.Graphics.DrawImage(m_provider.GetImageFor(user.Flags, stats), iconPosition);

                SizeF pingSize = e.Graphics.MeasureString(user.Ping.ToString(CultureInfo.CurrentCulture), e.Font);
                RectangleF pingArea = new RectangleF((float)e.Bounds.Right - 1.0f - pingSize.Width, (float)e.Bounds.Y + 1.0f,
                    pingSize.Width, pingSize.Height);
                e.Graphics.DrawString(user.Ping.ToString(CultureInfo.CurrentCulture), e.Font, textBrush, pingArea);

                SizeF nameSize = e.Graphics.MeasureString(user.Username, e.Font);
                RectangleF nameArea = new RectangleF((float)e.Bounds.X + (float)m_provider.IconSize.Width + 1.0f + 4.0f,
                    (float)e.Bounds.Y + (((float)e.Bounds.Height - nameSize.Height) / 2.0f),
                    (float)e.Bounds.Width - 10.0f - pingArea.Width - (float)m_provider.IconSize.Width,
                    (float)nameSize.Height);
                e.Graphics.DrawString(user.Username, e.Font, textBrush, nameArea, nameFormat);
            }
        }

        #endregion
    }
}
