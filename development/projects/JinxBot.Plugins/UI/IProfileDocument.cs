using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.UI
{
    public interface IProfileDocument
    {
        void AddDocument(DockableDocument profileDocument);
        void RemoveDocument(DockableDocument profileDocument);

        void AddToolWindow(DockableToolWindow toolWindow);
        void RemoveToolWindow(DockableToolWindow toolWindow);
    }
}
