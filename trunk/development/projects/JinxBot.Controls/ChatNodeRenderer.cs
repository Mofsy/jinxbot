using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Documents;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using GdiColor = System.Drawing.Color;
using WpfColor = System.Windows.Media.Color;

namespace JinxBot.Controls
{
    /// <summary>
    /// Implements a basic renderer for the general chat node.
    /// </summary>
    public class ChatNodeRenderer
    {
        /// <summary>
        /// Creates a new ChatNodeRenderer.
        /// </summary>
        public ChatNodeRenderer()
        {

        }

        public ResourceDictionary ResourceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Renders the specified chat node to the client.
        /// </summary>
        /// <param name="node">The node to append.</param>
        /// <remarks>
        /// <para>The return value of this function is a reference to the outermost <see cref="Inline">Inline</see> constructed
        /// by this function.  It may create additional inner elements as needed.</para>
        /// </remarks>
        /// <returns>
        /// Returns an object instance of <see cref="Inline">Inline</see> that can be appended to the HTML document.
        /// </returns>
        public virtual Inline Render(ChatNode node)
        {
            Inline result;

            if (node == ChatNode.NewLine)
            {
                result = new LineBreak();
            } 
            else if (node.LinkUri == null)
            {
                Run run = new Run();
                if (node.CssClass != null)
                {
                    run.Style = ResourceProvider[node.CssClass] as Style;
                    run.SetResourceReference(FrameworkContentElement.StyleProperty, node.CssClass);
                }
                else if (node.Color != GdiColor.Empty)
                {
                    run.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, node.Color.R, node.Color.G, node.Color.B));
                }
                run.Text = node.Text;

                result = run;
            }
            else // need to make a link.
            {
                Hyperlink link = new Hyperlink();
                link.Inlines.Add(new Run(node.Text));
                link.NavigateUri = node.LinkUri;

                if (node.CssClass != null)
                {
                    link.SetResourceReference(FrameworkContentElement.StyleProperty, node.CssClass);
                }
                else if (node.Color != GdiColor.Empty)
                {
                    link.Foreground = new SolidColorBrush(WpfColor.FromArgb(255, node.Color.R, node.Color.G, node.Color.B));
                }

                link.ToolTip = string.Format(CultureInfo.CurrentUICulture, "Link to {0}", node.LinkUri.ToString());

                result = link;
            }

            return result;
        }
    }
}
