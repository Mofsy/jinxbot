using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins;
using BNSharp.BattleNet.Stats;
using System.Drawing;
using System.Windows.Forms;
using BNSharp.BattleNet.Clans;
using WebIcon = JinxBot.Configuration.WebIconList.Icon;
using BNSharp;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JinxBot.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using BNSharp.BattleNet;
using JinxBot.Plugins.UI;

namespace JinxBot.Views.Chat
{
    internal class WebIconProvider : IIconProvider
    {
        private Dictionary<ClanRank, Image> m_ranksToImages;
        private Dictionary<UserFlags, Image> m_flagsToImages;
        private Dictionary<string, Image> m_nonTieredClientImages;
        private Image m_defaultTierIcon;
        // order: client, tier, race
        private Dictionary<string, Dictionary<int, Dictionary<char, Image>>> m_tieredClientImages;
        private static readonly Dictionary<Warcraft3IconRace, char> WARCRAFT_3_RACES_TO_IDS = new Dictionary<Warcraft3IconRace, char>
        {
            { Warcraft3IconRace.Human, 'H' },
            { Warcraft3IconRace.NightElf, 'N' },
            { Warcraft3IconRace.Orc, 'O' },
            { Warcraft3IconRace.Random, 'R' },
            { Warcraft3IconRace.Tournament, 'T' },
            { Warcraft3IconRace.Undead, 'U' }
        };

        private bool m_valid;
        private Image m_defaultImage;

        internal WebIconProvider()
        {
            WebIconList iconsList = null;

            Image target = new Bitmap(64, 44, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(target))
            using (Brush back = new SolidBrush(Color.Black))
            {
                g.FillRectangle(back, new Rectangle(Point.Empty, target.Size));
            }

            m_defaultImage = target;

            string xml = Resources.WebIconsList;
            using (StringReader sr = new StringReader(xml))
            using (XmlTextReader xtr = new XmlTextReader(sr))
            {
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(WebIconList));
                    iconsList = ser.Deserialize(xtr) as WebIconList;
                }
                catch (Exception ex)
                {
                    // TODO: Log the exception.
                    Trace.WriteLine(ex, "Exception loading in icon provider.");
                }
            }

            if (!object.ReferenceEquals(iconsList, null))
            {
                string localPath = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "Icons");

                m_valid = true;
                m_ranksToImages = new Dictionary<ClanRank, Image>();
                m_flagsToImages = new Dictionary<UserFlags, Image>();
                m_nonTieredClientImages = new Dictionary<string, Image>();
                m_tieredClientImages = new Dictionary<string, Dictionary<int, Dictionary<char, Image>>>();

                LoadClanIcons(iconsList, localPath);

                LoadFlagsIcons(iconsList, localPath);

                LoadNonTieredClientIcons(iconsList, localPath);

                LoadTieredClientIcons(iconsList, localPath);
            }
        }

        private Image LoadImageFromFile(string filePath)
        {
            Image result = new Bitmap(32, 22);
            using (Image src = Image.FromFile(filePath))
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.DrawImage(src, new Rectangle(Point.Empty, IconSize));
            }
            return result;
        }

        private void LoadTieredClientIcons(WebIconList iconsList, string localPath)
        {
            WebIcon defaultTierIcon = (from icon in iconsList.Icons
                                       where !string.IsNullOrEmpty(icon.ClientID) && icon.Tier == 1
                                       select icon).FirstOrDefault();

            if (!object.ReferenceEquals(defaultTierIcon, null))
            {
                try
                {
                    m_defaultImage = LoadImageFromFile(Path.Combine(localPath, defaultTierIcon.LocalName));
                }
                catch
                {
                    // TODO: Log the error or report it.
                }
            }

            var tieredClientIcons = from icon in iconsList.Icons
                                    where !string.IsNullOrEmpty(icon.ClientID) && icon.Tier > 1
                                    select icon;
            foreach (var tcIcon in tieredClientIcons)
            {
                // for tiered icons, assume each icon can only have one client.
                Image currentImage = null;
                try
                {
                    currentImage = LoadImageFromFile(Path.Combine(localPath, tcIcon.LocalName));
                }
                catch
                {
                    // TODO: Log the error or report it.
                    continue;
                }

                if (!m_tieredClientImages.ContainsKey(tcIcon.ClientID))
                    m_tieredClientImages.Add(tcIcon.ClientID, new Dictionary<int, Dictionary<char, Image>>());

                Dictionary<int, Dictionary<char, Image>> tierList = m_tieredClientImages[tcIcon.ClientID];

                if (!tierList.ContainsKey(tcIcon.Tier))
                    tierList.Add(tcIcon.Tier, new Dictionary<char, Image>());

                Dictionary<char, Image> raceList = tierList[tcIcon.Tier];
                raceList.Add(tcIcon.Race, currentImage);
            }
        }

        private void LoadNonTieredClientIcons(WebIconList iconsList, string localPath)
        {
            var nonTieredClientIcons = from icon in iconsList.Icons
                                       where !string.IsNullOrEmpty(icon.ClientID) && icon.Tier == 0
                                       select icon;

            foreach (var ntcIcon in nonTieredClientIcons)
            {
                string[] clients = ntcIcon.ClientID.Contains(",") ? ntcIcon.ClientID.Split(',') : new string[] { ntcIcon.ClientID };
                Image currentImage = null;
                try
                {
                    currentImage = LoadImageFromFile(Path.Combine(localPath, ntcIcon.LocalName));
                }
                catch
                {
                    // TODO: Log the error or report it.
                    continue;
                }

                foreach (string clientName in clients)
                {
                    m_nonTieredClientImages.Add(clientName, currentImage);
                }
            }
        }

        private void LoadFlagsIcons(WebIconList iconsList, string localPath)
        {
            var flagsIcons = from icon in iconsList.Icons
                             where !string.IsNullOrEmpty(icon.UserFlags)
                             select icon;

            foreach (var flagsIconEntry in flagsIcons)
            {
                string[] appropriateFlags = flagsIconEntry.UserFlags.Contains(",") ? flagsIconEntry.UserFlags.Split(',') : new string[] { flagsIconEntry.UserFlags };
                Image currentImage = null;
                try
                {
                    currentImage = LoadImageFromFile(Path.Combine(localPath, flagsIconEntry.LocalName));
                }
                catch
                {
                    // TODO: Log the error or report it.
                    continue;
                }

                foreach (string flagName in appropriateFlags)
                {
                    try
                    {
                        UserFlags actualFlag = (UserFlags)Enum.Parse(typeof(UserFlags), flagName);
                        m_flagsToImages.Add(actualFlag, currentImage);
                    }
                    catch
                    {
                        // TODO: Log the error or report it.
                        continue;
                    }
                }
            }
        }

        private void LoadClanIcons(WebIconList iconsList, string localPath)
        {
            var clanIcons = from icon in iconsList.Icons
                            where !string.IsNullOrEmpty(icon.ClanRank)
                            select icon;

            foreach (var clanIconEntry in clanIcons)
            {
                string[] appropriateRanks = clanIconEntry.ClanRank.Contains(",") ? clanIconEntry.ClanRank.Split(',') : new string[] { clanIconEntry.ClanRank };
                Image currentImage = null;
                try
                {
                    currentImage = LoadImageFromFile(Path.Combine(localPath, clanIconEntry.LocalName));
                }
                catch
                {
                    // TODO: Log the error or report it.
                    continue;
                }

                foreach (string rankName in appropriateRanks)
                {
                    try
                    {
                        ClanRank actualRank = (ClanRank)Enum.Parse(typeof(ClanRank), rankName);
                        m_ranksToImages.Add(actualRank, currentImage);
                    }
                    catch
                    {
                        // TODO: Log the error or report it.
                        continue;
                    }
                }
            }
        }

        #region IIconProvider Members
        [Obsolete]
        public ImageList GetImageList()
        {
            ImageList il = new ImageList();
            il.Images.Add(m_defaultImage);
            return il;
        }

        [Obsolete]
        public int GetImageIndexFor(UserStats stats)
        {
            return 0;
        }

        public Image GetImageFor(UserFlags flags, UserStats stats)
        {
            foreach (UserFlags flag in m_flagsToImages.Keys)
            {
                if (TestFlag(flag, flags))
                {
                    return m_flagsToImages[flag];
                }
            }

            Image clientBasedImage = null;
            if (m_nonTieredClientImages.ContainsKey(stats.Product.ProductCode))
                clientBasedImage = m_nonTieredClientImages[stats.Product.ProductCode];

            Warcraft3Stats w3 = stats as Warcraft3Stats;
            if (!object.ReferenceEquals(w3, null))
            {
                if (w3.IconTier == 1)
                {
                    clientBasedImage = m_defaultTierIcon;
                }
                else
                {
                    if (m_tieredClientImages.ContainsKey(stats.Product.ProductCode))
                    {
                        Dictionary<int, Dictionary<char, Image>> tiers = m_tieredClientImages[stats.Product.ProductCode];
                        if (tiers.ContainsKey(w3.IconTier))
                        {
                            Dictionary<char, Image> races = tiers[w3.IconTier];
                            if (WARCRAFT_3_RACES_TO_IDS.ContainsKey(w3.IconRace))
                            {
                                char race = WARCRAFT_3_RACES_TO_IDS[w3.IconRace];
                                if (races.ContainsKey(race))
                                {
                                    clientBasedImage = races[race];
                                }
                            }
                        }
                    }  
                }
            }

            if (object.ReferenceEquals(clientBasedImage, null))
                clientBasedImage = m_defaultImage;

            return clientBasedImage;
        }

        private bool TestFlag(UserFlags flags, UserFlags userFlags)
        {
            return ((flags & userFlags) != UserFlags.None);
        }

        [Obsolete]
        public ImageList GetClanImageList()
        {
            ImageList il = new ImageList();
            foreach (ClanRank rank in m_ranksToImages.Keys)
            {
                il.Images.Add(m_ranksToImages[rank]);
            }
            return il;
        }

        [Obsolete]
        public int GetImageIndexForClanRank(ClanRank rank)
        {
            return 0;
        }

        public Image GetImageFor(ClanRank rank)
        {
            if (m_ranksToImages.ContainsKey(rank))
            {
                return m_ranksToImages[rank];
            }

            return m_defaultImage;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!object.ReferenceEquals(m_defaultImage, null))
                {
                    m_defaultImage.Dispose();
                    m_defaultImage = null;
                }

                if (!object.ReferenceEquals(m_defaultTierIcon, null))
                {
                    m_defaultTierIcon.Dispose();
                    m_defaultTierIcon = null;
                }

                if (!object.ReferenceEquals(m_flagsToImages, null))
                {
                    foreach (Image img in m_flagsToImages.Values)
                    {
                        img.Dispose();
                    }
                    m_flagsToImages.Clear();
                    m_flagsToImages = null;
                }

                if (!object.ReferenceEquals(m_nonTieredClientImages, null))
                {
                    foreach (Image img in m_nonTieredClientImages.Values)
                    {
                        img.Dispose();
                    }
                    m_nonTieredClientImages.Clear();
                    m_nonTieredClientImages = null;
                }

                if (!object.ReferenceEquals(m_ranksToImages, null))
                {
                    foreach (Image img in m_ranksToImages.Values)
                    {
                        img.Dispose();
                    }
                    m_ranksToImages.Clear();
                    m_ranksToImages = null;
                }

                if (!object.ReferenceEquals(m_tieredClientImages, null))
                {
                    foreach (Dictionary<int, Dictionary<char, Image>> tierSet in m_tieredClientImages.Values)
                    {
                        foreach (Dictionary<char, Image> raceSet in tierSet.Values)
                        {
                            foreach (Image img in raceSet.Values)
                            {
                                img.Dispose();
                            }
                            raceSet.Clear();
                        }
                        tierSet.Clear();
                    }
                    m_tieredClientImages.Clear();
                    m_tieredClientImages = null;
                }
            }
        }

        #endregion

        #region IIconProvider Members


        public string GetImageIdFor(UserFlags flags, UserStats us)
        {
            foreach (UserFlags flag in m_flagsToImages.Keys)
            {
                if (TestFlag(flag, flags))
                {
                    return flag.ToString();
                }
            }

            string clientBasedImage = us.Product.ProductCode;

            Warcraft3Stats w3 = us as Warcraft3Stats;
            if (!object.ReferenceEquals(w3, null))
            {
                if (w3.IconTier == 1)
                {
                    clientBasedImage = "W3O1";
                }
                else
                {
                    clientBasedImage = string.Format("{0}{1}{2}", w3.Product == Product.Warcraft3Retail ? "W3" : "FT", w3.IconRace.ToString(), w3.IconTier);
                }
            }

            Trace.WriteLine(clientBasedImage, "Determined Image ID");

            return clientBasedImage;
        }

        public Size IconSize
        {
            get { return new Size(32, 22); }
        }

        #endregion
    }
}
