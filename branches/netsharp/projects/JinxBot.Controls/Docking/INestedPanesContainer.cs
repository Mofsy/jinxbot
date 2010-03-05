using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace JinxBot.Controls.Docking
{
#pragma warning disable 1591
    public interface INestedPanesContainer
    {
        DockState DockState { get; }
        Rectangle DisplayingRectangle { get; }
        NestedPaneCollection NestedPanes { get; }
        VisibleNestedPaneCollection VisibleNestedPanes { get; }
        bool IsFloat { get; }
    }
#pragma warning restore 1591
}
