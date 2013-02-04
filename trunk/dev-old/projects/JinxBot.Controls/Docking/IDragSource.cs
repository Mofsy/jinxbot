using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace JinxBot.Controls.Docking
{
    internal interface IDragSource
    {
        Control DragControl { get; }
    }
}
