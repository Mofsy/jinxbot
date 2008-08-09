using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BNSharp.BattleNet.Stats;
using BNSharp.BattleNet.Clans;
using BNSharp;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Provides an icon list for use in the channel list.
    /// </summary>
    public interface IIconProvider : IDisposable
    {
        /// <summary>
        /// Gets the image associated with the specified stats.
        /// </summary>
        /// <param name="stats">The stats for which to retrieve the image.</param>
        /// <returns>An image corresponding to the user's product, stats, or flags, as appropriate.</returns>
        Image GetImageFor(UserFlags flags, UserStats stats);

        /// <summary>
        /// Gets the image associated with the specified clan rank.
        /// </summary>
        /// <param name="rank">The rank to check.</param>
        /// <returns>An image corresponding to the user's rank.</returns>
        Image GetImageFor(ClanRank rank);

        /// <summary>
        /// Gets a unique ID for the an image associated 
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        string GetImageIdFor(ClanRank rank);

        string GetImageIdFor(Product product);

        Image GetImageFor(Product product);

        string GetImageIdFor(UserFlags flags, UserStats us);

        Size IconSize { get; }
    }
}
