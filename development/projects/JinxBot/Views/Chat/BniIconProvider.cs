using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins;
using System.Windows.Forms;
using JinxBot.Configuration;
using System.IO;
using BNSharp.MBNCSUtil.Net;
using BNSharp.BattleNet;
using System.Drawing;
using BNSharp.MBNCSUtil.Data;
using BNSharp.BattleNet.Stats;
using BNSharp.BattleNet.Clans;
using JinxBot.Plugins.UI;

namespace JinxBot.Views.Chat
{
    /// <summary>
    /// Implements the <see>IIconProvider</see> interface for use with icons.bni.
    /// </summary>
    public class BniIconProvider : IIconProvider, IDisposable
    {
        private bool m_valid;
        private BniFileParser m_bni;
        private Bitmap m_fail;

        public BniIconProvider()
        {
            string basePath = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "icons.bni");
            if (!File.Exists(basePath))
            {
                try
                {
                    BnFtpRequestBase req = new BnFtpVersion1Request(Product.StarcraftRetail.ProductCode, "icons.bni", null);
                    req.LocalFileName = basePath;
                    req.ExecuteRequest();

                    m_bni = new BniFileParser(basePath);
                    m_valid = true;
                }
                catch
                {
                    CreateFailedBitmap();
                }
            }
            else
            {
                try
                {
                    m_bni = new BniFileParser(basePath);
                    m_valid = true;
                }
                catch
                {
                    CreateFailedBitmap();
                }
            }
        }

        private void CreateFailedBitmap()
        {
            Bitmap bmp = new Bitmap(28, 14);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush b = new SolidBrush(Color.Black))
            {
                g.FillRectangle(b, new Rectangle(Point.Empty, bmp.Size));
            }

            m_fail = bmp;
        }

        #region IIconProvider Members

        /// <summary>
        /// Gets an image list containing all icons supported by this icon provider.
        /// </summary>
        /// <returns>An <see>ImageList</see> for use in the channel list.</returns>
        public ImageList GetImageList()
        {
            ImageList list = new ImageList();
            list.ColorDepth = ColorDepth.Depth32Bit;
            list.ImageSize = new System.Drawing.Size(28, 14);

            if (m_valid)
            {
                foreach (BniIcon icon in m_bni.AllIcons)
                {
                    list.Images.Add(icon.Image);
                }
            }
            else
            {
                list.Images.Add(m_fail);
            }

            return list;
        }

        /// <summary>
        /// Gets the image index into the image list for the user with the specified stats.
        /// </summary>
        /// <param name="stats">The stats for which to calculate the image index.</param>
        /// <returns>An index valid within the <see>ImageList</see> returned from <see>GetImageList</see>.</returns>
        public int GetImageIndexFor(UserStats stats)
        {
            if (m_valid)
            {
                BniIcon defaultIcon = (from icon in m_bni.AllIcons
                                       where icon.SoftwareProductCodes.Length > 0 && icon.SoftwareProductCodes[0] == stats.Product.ProductCode
                                       select icon).FirstOrDefault();
                int defaultIndex = 0;
                for (int i = 0; i < m_bni.AllIcons.Length; i++)
                {
                    if (m_bni.AllIcons[i] == defaultIcon)
                    {
                        defaultIndex = i;
                        break;
                    }
                }

                // TO DO:
                // More advanced image index lookup.

                return defaultIndex;
            }
            else
            {
                return 0;
            }
        }

        public Image GetImageFor(UserStats stats)
        {
            if (m_valid)
                return m_bni.AllIcons[GetImageIndexFor(stats)].Image;
            else
                return m_fail;
        }

        public ImageList GetClanImageList()
        {
            ImageList list = new ImageList();
            list.ImageSize = new Size(28, 14);
            list.ColorDepth = ColorDepth.Depth32Bit;

            if (m_valid)
            {
                list.Images.Add(m_bni.AllIcons[2].Image);
                list.Images.Add(m_bni.AllIcons[3].Image);
                list.Images.Add(m_bni.AllIcons[4].Image);
                list.Images.Add(m_bni.AllIcons[19].Image);
                list.Images.Add(m_bni.AllIcons[18].Image);
            }
            else
            {
                list.Images.Add(m_fail);
            }

            return list;
        }

        public int GetImageIndexForClanRank(ClanRank rank)
        {
            if (rank < ClanRank.Initiate || rank > ClanRank.Chieftan)
                rank = ClanRank.Initiate;

            if (m_valid)
            {
                return ((int)ClanRank.Chieftan - (int)rank);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, cleaning up unmanaged and managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object, cleaning up unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">Whether to clean managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_bni != null)
                {
                    m_bni.Dispose();
                    m_bni = null;
                }
            }

            m_valid = false;
        }
        #endregion
    }
}
