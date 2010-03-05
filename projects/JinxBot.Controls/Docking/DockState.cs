using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Specifies the docking state of a docking control.
    /// </summary>
    public enum DockState
    {
        /// <summary>
        /// Specifies that it is in an unknown or invalid state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Specifies that it is floating.
        /// </summary>
        Float = 1,
        /// <summary>
        /// Specifies that it is at the top but is auto-hiding.
        /// </summary>
        DockTopAutoHide = 2,
        /// <summary>
        /// Specifies that it is at the left but is auto-hiding.
        /// </summary>
        DockLeftAutoHide = 3,
        /// <summary>
        /// Specifies that it is at the bottom but is auto-hiding.
        /// </summary>
        DockBottomAutoHide = 4,
        /// <summary>
        /// Specifies that it is at the right but is auto-hiding.
        /// </summary>
        DockRightAutoHide = 5,
        /// <summary>
        /// Specifies that it is a document.
        /// </summary>
        Document = 6,
        /// <summary>
        /// Specifies that is it docked on the top.
        /// </summary>
        DockTop = 7,
        /// <summary>
        /// Specifies that it is docked on the left.
        /// </summary>
        DockLeft = 8,
        /// <summary>
        /// Specifies that it is docked on the bottom.
        /// </summary>
        DockBottom = 9,
        /// <summary>
        /// Specifies that it is docked on the right.
        /// </summary>
        DockRight = 10,
        /// <summary>
        /// Specifies that it is hidden.
        /// </summary>
        Hidden = 11
    }
}
