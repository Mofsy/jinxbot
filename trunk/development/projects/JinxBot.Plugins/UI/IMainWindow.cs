using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Provides document functionality to the main JinxBot window, allowing a user to create or remove dockable windows to the main user interface.
    /// </summary>
    public interface IMainWindow
    {
        /// <summary>
        /// Adds a main-window document to the user interface.
        /// </summary>
        /// <param name="mainWindowDocument"></param>
        void AddDocument(DockableDocument mainWindowDocument);
        /// <summary>
        /// Removes a previously-created main window document from the user interface.
        /// </summary>
        /// <param name="mainWindowDocument">The main-window document.</param>
        void RemoveDocument(DockableDocument mainWindowDocument);
    }
}
