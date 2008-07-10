using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls;
using System.Windows.Forms;
using System.Globalization;

namespace JinxBot.Views.Chat
{
    internal class ImageChatNodeRenderer : ChatNodeRenderer
    {
        /// <summary>
        /// Renders the specified chat node to the client.
        /// </summary>
        /// <param name="node">The node to append.</param>
        /// <remarks>
        /// <para>The return value of this function is a reference to the outermost <see cref="HtmlElement">HtmlElement</see> constructed
        /// by this function.  It may create additional inner elements as needed.</para>
        /// </remarks>
        /// <returns>
        /// Returns an object instance of <see cref="HtmlElement">HtmlElement</see> that can be appended to the HTML document.
        /// </returns>
        public override HtmlElement Render(ChatNode node)
        {
            ImageChatNode icn = node as ImageChatNode;
            if (icn != null)
            {
                HtmlElement img = base.HtmlDomDocument.CreateElement("img");
                img.SetAttribute("src", string.Concat(ImageChatNodeProtocol.Schema, ":", icn.ImageName));
                img.SetAttribute("alt", icn.Text);

                if (icn.LinkUri != null)
                {
                    HtmlElement hrefSection = base.HtmlDomDocument.CreateElement("a");
                    hrefSection.SetAttribute("href", node.LinkUri.ToString());

                    hrefSection.SetAttribute("title",
                        string.Format(CultureInfo.CurrentCulture, "Link to {0}", node.LinkUri.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")));

                    hrefSection.AppendChild(img);
                    hrefSection.Click += new HtmlElementEventHandler(hrefSection_Click);

                    return hrefSection;
                }

                return img;
            }
            else
            {
                return base.Render(node);
            }
        }
    }
}
