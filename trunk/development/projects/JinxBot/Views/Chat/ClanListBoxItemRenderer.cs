using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins;
using JinxBot.Plugins.UI;
using BNSharp.BattleNet.Clans;
using System.Drawing;

namespace JinxBot.Views.Chat
{
    public class ClanListBoxItemRenderer : ICustomListBoxItemRenderer
    {
        #region ICustomListBoxItemRenderer Members

        public void MeasureItem(CustomMeasurements e)
        {
            e.ItemHeight = 48;
        }

        public void DrawItem(CustomItemDrawData e)
        {
            e.DrawBackground();
            if (e.State == System.Windows.Forms.DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }

            ClanMember clanMember = e.Item as ClanMember;
            using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
            {
                IIconProvider prov = ProfileResourceProvider.GetForClient(null).Icons;
                e.Graphics.DrawImage(prov.GetClanImageList().Images[prov.GetImageIndexForClanRank(clanMember.Rank)], (PointF)e.Bounds.Location);

                RectangleF nameArea = new RectangleF((float)e.Bounds.X + 64f, (float)e.Bounds.Y, (float)e.Bounds.Width - 64f, (float)e.Bounds.Height);
                e.Graphics.DrawString(clanMember.Username, new Font("Calibri", 12, GraphicsUnit.Point), textBrush, nameArea);
            }
        }

        #endregion
    }
}
