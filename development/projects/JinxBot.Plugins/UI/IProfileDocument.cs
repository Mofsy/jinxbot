using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.Docking;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Represents the component functionality of a JinxBot profile document.
    /// </summary>
    /// <remarks>
    /// <para>JinxBot profiles are each visualized within the JinxBot overall user interface as docking-enabled document 
    /// windows, similar to the Visual Studio user interface.  Within these profile visualizations are the capabilities of 
    /// displaying sub-documents (derived from <see>DockableDocument</see>) and tool windows (derived from
    /// <see>DockableToolWindow</see>).  In order to add these windows, a plugin author must use the JinxBot.Controls library.</para>
    /// </remarks>
    public interface IProfileDocument
    {
        /// <summary>
        /// Adds a sub-document to a profile document container.
        /// </summary>
        /// <param name="profileDocument">The profile document to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="profileDocument"/> is <see langword="null" />.</exception>
        void AddDocument(DockableDocument profileDocument);
        /// <summary>
        /// Removes a sub-document from a profile document container.
        /// </summary>
        /// <remarks>
        /// <para>If the sub-document was not part of the profile document container, this method has no effect.</para>
        /// </remarks>
        /// <param name="profileDocument">The profile document reference to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="profileDocument"/> is <see langword="null" />.</exception>
        void RemoveDocument(DockableDocument profileDocument);


        /// <summary>
        /// Adds a tool window to a profile document container.
        /// </summary>
        /// <param name="toolWindow">The tool window to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ToolWindow"/> is <see langword="null" />.</exception>
        void AddToolWindow(DockableToolWindow toolWindow);
        /// <summary>
        /// Removes a tool window from a profile document container.
        /// </summary>
        /// <param name="toolWindow">The tool window to remove.</param>
        /// <remarks>
        /// <para>If the tool window was not part of the profile document container, this method has no effect.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ToolWindow"/> is <see langword="null" />.</exception>
        void RemoveToolWindow(DockableToolWindow toolWindow);
    }
}
