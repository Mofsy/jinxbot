using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Specifies the document style of this window.
    /// </summary>
    public enum DocumentStyle
    {
        /// <summary>
        /// Specifies that it should use the MDI document interface (such as Visual Studio).
        /// </summary>
        DockingMdi,
        /// <summary>
        /// Specifies that it should use a window MDI document interface.
        /// </summary>
        DockingWindow,
        /// <summary>
        /// Specifies that it should use a single document interface with docking.
        /// </summary>
        DockingSdi,
        /// <summary>
        /// Specifies that multiple document interface should use the system implementation.
        /// </summary>
        SystemMdi,
    }
}
