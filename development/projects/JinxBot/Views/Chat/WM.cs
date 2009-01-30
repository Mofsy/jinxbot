using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Views.Chat
{
    internal enum WM
    {
        Destroy = 2,
        KeyDown = 0x100,
        KeyUp = 0x101,
        Char = 0x102,
        SysChar = 0x106,
    }
}
