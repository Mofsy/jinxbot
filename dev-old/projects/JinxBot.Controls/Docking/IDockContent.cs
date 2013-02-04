using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Specifies a docking content.
    /// </summary>
    public interface IDockContent
    {
        /// <summary>
        /// Gets the docking handler associated with this object.
        /// </summary>
        DockContentHandler DockHandler { get; }
    }
}
