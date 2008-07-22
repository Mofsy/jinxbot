using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BNSharp.BattleNet.Stats;
using BNSharp.BattleNet.Clans;
using BNSharp;

namespace JinxBot.Plugins
{
    /// <summary>
    /// Provides an icon list for use in the channel list.
    /// </summary>
    public interface IIconProvider : IDisposable
    {
        /// <summary>
        /// Gets an image list containing all icons supported by this icon provider.
        /// </summary>
        /// <returns>An <see>ImageList</see> for use in the channel list.</returns>
        [Obsolete]
        ImageList GetImageList();

        /// <summary>
        /// Gets the image index into the image list for the user with the specified stats.
        /// </summary>
        /// <param name="stats">The stats for which to calculate the image index.</param>
        /// <returns>An index valid within the <see>ImageList</see> returned from <see>GetImageList</see>.</returns>
        [Obsolete]
        int GetImageIndexFor(UserStats stats);

        /// <summary>
        /// Gets the image associated with the specified stats.
        /// </summary>
        /// <param name="stats">The stats for which to retrieve the image.</param>
        /// <returns>An image corresponding to the user's product.</returns>
        Image GetImageFor(UserFlags flags, UserStats stats);

        /// <summary>
        /// Gets the clan image list.
        /// </summary>
        /// <returns>An ImageList.</returns>
        [Obsolete]
        ImageList GetClanImageList();

        /// <summary>
        /// Gets the index of the image in the ImageList from <see>GetClanImageList</see> for the specified clan rank.
        /// </summary>
        /// <param name="rank">The rank for which to get the image index.</param>
        /// <returns>An index into the ImageList returned by <see>GetClanImageList</see>.</returns>
        [Obsolete]
        int GetImageIndexForClanRank(ClanRank rank);

        /// <summary>
        /// Gets a reference to an image used for a specified clan rank.
        /// </summary>
        /// <param name="rank">The rank for which to get the image.</param>
        /// <returns>An image corresponding to the clan image.</returns>
        Image GetImageFor(ClanRank rank);

        /// <summary>
        /// Gets a unique ID for an image.
        /// </summary>
        /// <param name="userFlags"></param>
        /// <param name="us"></param>
        /// <returns></returns>
        string GetImageIdFor(UserFlags userFlags, UserStats us);
    }
}
