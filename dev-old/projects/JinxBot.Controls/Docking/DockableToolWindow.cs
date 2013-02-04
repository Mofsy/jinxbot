using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Provides automatic support as a tool window.  A tool window by default can only dock to the sides or float, and the close button is 
    /// disabled.
    /// </summary>
    public class DockableToolWindow : DockContent
    {
        /// <summary>
        /// Creates a new DockableToolWindow.
        /// </summary>
        public DockableToolWindow()
        {
            this.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Float;
            this.CloseButton = false;
        }
    }
}
