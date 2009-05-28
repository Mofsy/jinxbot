using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Enables a custom renderer for JinxBot's list boxes, including the user list, friends list, and clan list.
    /// </summary>
    public interface ICustomListBoxItemRenderer
    {
        /// <summary>
        /// Instructs the renderer to calculate the dimensions of an item being rendered.
        /// </summary>
        /// <param name="e">The measurements arguments.</param>
        void MeasureItem(CustomMeasurements e);

        /// <summary>
        /// Instructs the renderer to perform the actual drawing.
        /// </summary>
        /// <param name="e">The drawing arguments.</param>
        void DrawItem(CustomItemDrawData e);
    }
}
