using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Provides a shorthand class that can be subclassed to behave only as a document.
    /// </summary>
    public class DockableDocument : DockContent
    {
        /// <summary>
        /// Creates a new <see>DockableDocument</see>.
        /// </summary>
        public DockableDocument()
        {
            this.DockAreas = DockAreas.Document | DockAreas.Float;
        }
    }
}
