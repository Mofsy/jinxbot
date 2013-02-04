using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace JinxBot.Controls.Docking
{
    internal class DummyControl : Control
    {
        public DummyControl()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
