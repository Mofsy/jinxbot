using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;

namespace JinxBot.Windows7
{
    internal interface IJumpListWindowTarget : IMainWindow
    {
        void HandleJumpListCall(string[] param);
    }
}
