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
using BNSharp;

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
            CreateFailedBitmap();

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
                catch { }
            }
            else
            {
                try
                {
                    m_bni = new BniFileParser(basePath);
                    m_valid = true;
                }
                catch { }
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

        #region IIconProvider Members
        public Image GetImageFor(UserFlags flags, UserStats stats)
        {
            UserFlags[] allFlags = new UserFlags[] { UserFlags.BlizzardRepresentative, UserFlags.BattleNetAdministrator, UserFlags.ChannelOperator, UserFlags.Speaker, UserFlags.SpecialGuest, UserFlags.Squelched, UserFlags.GFOfficial, UserFlags.GFPlayer };

            foreach (UserFlags flag in allFlags)
            {
                if (TestFlag(flags, flag))
                {
                    BniIcon ico = (from icon in m_bni.AllIcons
                                    where (icon.UserFlags & flag) == flag
                                    select icon).FirstOrDefault();
                    if (!object.ReferenceEquals(null, ico))
                        return ico.Image;
                }
            }

            BniIcon img = (from icon in m_bni.AllIcons
                            where icon.SoftwareProductCodes.Contains(stats.Product.ProductCode)
                            select icon).FirstOrDefault();

            if (!object.ReferenceEquals(null, img))
                return img.Image;

            return m_fail;
        }

        private static bool TestFlag(UserFlags flags, UserFlags flag)
        {
            return ((flags & flag) == flag);
        }

        public Image GetImageFor(ClanRank rank)
        {
            switch (rank)
            {
                case ClanRank.Chieftan:
                    return GetImageFor(UserFlags.ChannelOperator, null);
                case ClanRank.Shaman:
                    return GetImageFor(UserFlags.Speaker, null);
                case ClanRank.Grunt:
                    return GetImageFor(UserFlags.None, UserStats.CreateDefault(Product.Warcraft3Expansion));
                case ClanRank.Peon:
                case ClanRank.Initiate:
                    return GetImageFor(UserFlags.None, UserStats.CreateDefault(Product.Warcraft3Retail));
                default:
                    return GetImageFor(UserFlags.BattleNetAdministrator, null);
            }
        }

        public string GetImageIdFor(UserFlags flags, UserStats us)
        {
            UserFlags[] allFlags = new UserFlags[] { UserFlags.BlizzardRepresentative, UserFlags.BattleNetAdministrator, UserFlags.ChannelOperator, UserFlags.Speaker, UserFlags.SpecialGuest, UserFlags.Squelched, UserFlags.GFOfficial, UserFlags.GFPlayer };

            foreach (UserFlags flag in allFlags)
            {
                if (TestFlag(flags, flag))
                {
                    BniIcon ico = (from icon in m_bni.AllIcons
                                   where (icon.UserFlags & flag) == flag
                                   select icon).FirstOrDefault();
                    if (!object.ReferenceEquals(null, ico))
                        return flag.ToString();
                }
            }

            return us.Product.ProductCode;
        }

        public Size IconSize
        {
            get
            {
                return new Size(28, 14);
            }
        }

        public string GetImageIdFor(ClanRank rank)
        {
            return rank.ToString();
        }

        public string GetImageIdFor(Product product)
        {
            BniIcon img = (from icon in m_bni.AllIcons
                           where icon.SoftwareProductCodes.Contains(product.ProductCode)
                           select icon).FirstOrDefault();

            if (!object.ReferenceEquals(null, img))
                return product.ProductCode;

            return string.Empty;
        }

        public Image GetImageFor(Product product)
        {
            if (object.ReferenceEquals(product, null))
                return m_fail;

            BniIcon img = (from icon in m_bni.AllIcons
                           where icon.SoftwareProductCodes.Contains(product.ProductCode)
                           select icon).FirstOrDefault();

            if (!object.ReferenceEquals(null, img))
                return img.Image;

            return m_fail;
        }

        #endregion
    }
}
