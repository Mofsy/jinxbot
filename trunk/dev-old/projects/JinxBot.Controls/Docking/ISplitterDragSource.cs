using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace JinxBot.Controls.Docking
{
    internal interface ISplitterDragSource : IDragSource
    {
        void BeginDrag(Rectangle rectSplitter);
        void EndDrag();
        bool IsVertical { get; }
        Rectangle DragLimitBounds { get; }
        void MoveSplitter(int offset);
    }
}
