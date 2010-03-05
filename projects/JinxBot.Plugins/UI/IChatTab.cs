using System;


namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// Represents the functionality exposed by a chat tab.
    /// </summary>
    public interface IChatTab
    {
        /// <summary>
        /// Gets or sets the URI where the stylesheet is located.
        /// </summary>
        Uri StylesheetUri { get; set; }
    }
}
