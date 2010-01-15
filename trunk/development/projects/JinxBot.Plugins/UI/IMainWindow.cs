using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Provides document functionality to the main JinxBot window, allowing a user to create or remove dockable 
    /// windows to the main user interface.
    /// </summary>
    /// <remarks>
    /// <para>JinxBot profiles are each visualized within the JinxBot overall user interface as docking-enabled document 
    /// windows, similar to the Visual Studio user interface.  Within these profile visualizations are the capabilities of 
    /// displaying sub-documents (derived from <see>DockableDocument</see>) and tool windows (derived from
    /// <see>DockableToolWindow</see>).  In order to add these windows, a plugin author must use the JinxBot.Controls library.</para>
    /// <para>
    ///     
    /// </para>
    /// </remarks>
    public interface IMainWindow
    {
        /// <summary>
        /// Adds a main-window document to the user interface.
        /// </summary>
        /// <remarks>
        /// <para>Future versions of JinxBot will track which plugins add which documents, and will only allow addition or 
        /// removal of documents at the specified loading or closing times.  This will add security to the initialization
        /// routines, and prevent plugins from interfering with each other.</para>
        /// </remarks>
        /// <param name="mainWindowDocument">The document to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mainWindowDocument"/> is <see langword="null"/>.</exception>
        void AddDocument(DockableDocument mainWindowDocument);
        /// <summary>
        /// Removes a previously-created main window document from the user interface.
        /// </summary>
        /// <remarks>
        /// <para>Future versions of JinxBot will track which plugins add which documents, and will only allow addition or 
        /// removal of documents at the specified loading or closing times.  This will add security to the initialization
        /// routines, and prevent plugins from interfering with each other.</para>
        /// <para>If the specified <paramref name="mainWindowDocument"/> document is not already a part of the main window's
        /// children, calling this method will have no effect.</para>
        /// </remarks>
        /// <param name="mainWindowDocument">The main-window document.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mainWindowDocument"/> is <see langword="null" />.</exception>
        void RemoveDocument(DockableDocument mainWindowDocument);
    }
}
