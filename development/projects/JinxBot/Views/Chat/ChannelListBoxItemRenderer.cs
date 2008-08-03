using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;
using BNSharp;
using System.Drawing;
using BNSharp.BattleNet.Stats;
using System.Globalization;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using BNSharp.BattleNet;

namespace JinxBot.Views.Chat
{
    public class ChannelListBoxItemRenderer : ICustomListBoxItemRenderer, IDisposable
    {
        private IIconProvider m_provider;
        private Bitmap m_pingImg;

        internal ChannelListBoxItemRenderer()
        {
            ProfileResourceProvider prp = ProfileResourceProvider.GetForClient(null);
            if (!object.ReferenceEquals(prp, null))
                m_provider = prp.Icons;

            m_pingImg = new Bitmap(28, 14);
            Rectangle bounds = new Rectangle(0, 0, 28, 14);
            using (Graphics g = Graphics.FromImage(m_pingImg))
            using (LinearGradientBrush b = new LinearGradientBrush(bounds, Color.Black, Color.Black, 0, false))
            {
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { Color.LimeGreen, Color.Lime, Color.Yellow, Color.Orange, Color.OrangeRed, Color.Maroon };
                cb.Positions = new float[] { 0f, 0.1f, 0.4f, 0.6f, 0.9f, 1f };
                b.InterpolationColors = cb;

                g.FillRectangle(b, bounds);
            }
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

            ChatUser user = e.Item as ChatUser;
            using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
            using (StringFormat nameFormat = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter })
            //using (StringFormat pingFormat = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter, Alignment = StringAlignment.Center })
            using (SolidBrush pingOutlineColor = new SolidBrush(Color.DarkRed))
            using (Pen pingOutline = new Pen(pingOutlineColor))
            {
                UserStats stats = user.Stats;
                PointF iconPosition = new PointF((float)e.Bounds.Left + 1.0f, (float)e.Bounds.Top + 1.0f);
                e.Graphics.DrawImage(m_provider.GetImageFor(user.Flags, stats), iconPosition);

                int userPing = user.Ping;
                if (userPing < 0)
                    userPing = int.MaxValue;
                int pingWidth = (int)Math.Min(28.0f * ((float)userPing / 600.0f), 28.0f);
                Rectangle pingImgArea = new Rectangle(e.Bounds.Right - 33, e.Bounds.Y + (e.Bounds.Height - m_pingImg.Height) / 2,
                    pingWidth, m_pingImg.Height);
                e.Graphics.DrawImageUnscaledAndClipped(m_pingImg, pingImgArea);

                //Rectangle pingOutlineBox = new Rectangle(pingImgArea.X, pingImgArea.Y, m_pingImg.Width - 1, m_pingImg.Height - 1);
                //e.Graphics.DrawRectangle(pingOutline, pingOutlineBox);

                //string userPingText = user.Ping.ToString(CultureInfo.CurrentCulture);
                //e.Graphics.DrawString(userPingText, e.Font, textBrush, pingOutlineBox, pingFormat);

                SizeF nameSize = e.Graphics.MeasureString(user.Username, e.Font);
                RectangleF nameArea = new RectangleF((float)e.Bounds.X + (float)m_provider.IconSize.Width + 1.0f + 4.0f,
                    (float)e.Bounds.Y + (((float)e.Bounds.Height - nameSize.Height) / 2.0f),
                    (float)e.Bounds.Width - 10.0f - 28.0f - (float)m_provider.IconSize.Width,
                    (float)nameSize.Height);
                e.Graphics.DrawString(user.Username, e.Font, textBrush, nameArea, nameFormat);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
