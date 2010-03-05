using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using JinxBot.Controls.Docking.Design;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Specifies the locations that can be set as a docking location.
    /// </summary>
    [Flags]
    [Serializable]
    [Editor(typeof(DockAreasEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public enum DockAreas
    {
        /// <summary>
        /// Specifies no docking position.  This field is invalid.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the item should float.
        /// </summary>
        Float = 1,
        /// <summary>
        /// Specifies that the item should be docked at the left of the docking area.
        /// </summary>
        DockLeft = 2,
        /// <summary>
        /// Specifies that the item should be docked at the right of the docking area.
        /// </summary>
        DockRight = 4,
        /// <summary>
        /// Specifies that the item should be docked at the top of the docking area.
        /// </summary>
        DockTop = 8,
        /// <summary>
        /// Specifies that the item should be docked at the bottom of the docking area.
        /// </summary>
        DockBottom = 16,
        /// <summary>
        /// Specifies that the item should be docked as a document.
        /// </summary>
        Document = 32
    }
}
