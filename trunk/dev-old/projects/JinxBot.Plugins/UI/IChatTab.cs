using System;
using System.Collections.Generic;
using JinxBot.Controls;


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

        /// <summary>
        /// Adds the specified chat nodes as a single paragraph to the chat display.
        /// </summary>
        /// <param name="nodes">The chat nodes to add.</param>
        void AddChat(IEnumerable<ChatNode> nodes);
    }
}
