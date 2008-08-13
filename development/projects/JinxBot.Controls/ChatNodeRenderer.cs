using System;
using System.Collections.Generic;
using System.Text;
using mshtml;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;
using System.Globalization;

namespace JinxBot.Controls
{
    /// <summary>
    /// Implements a basic renderer for the general chat node.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    public class ChatNodeRenderer
    {
        private HtmlDocument m_domDoc;
        /// <summary>
        /// Creates a new ChatNodeRenderer.
        /// </summary>
        public ChatNodeRenderer()
        {

        }

        /// <summary>
        /// Gets or sets the DOM document that should be used for creating HTML entities.
        /// </summary>
        /// <remarks>
        /// <para>Because the ChatBox uses an HTML control for the display of complex chat node messages, a renderer may require use
        /// of the HTML DOM for complex effects.  When a ChatNodeRenderer is created, then, it receives a reference to the HTML DOM
        /// before it is asked to render a node.</para>
        /// </remarks>
        public HtmlDocument HtmlDomDocument
        {
            get { return m_domDoc; }
            set 
            {
                Contract.RequireInstance(value, "value");

                m_domDoc = value; 
            }
        }

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
        public virtual HtmlElement Render(ChatNode node)
        {
            HtmlElement result;

            if (node == ChatNode.NewLine)
            {
                HtmlElement lineBreak = this.m_domDoc.CreateElement("br");
                result = lineBreak;
            } 
            else if (node.LinkUri == null)
            {
                HtmlElement chatSection = this.m_domDoc.CreateElement("span");
                if (node.CssClass != null)
                {
                    chatSection.SetAttribute("className", node.CssClass);
                }
                else if (node.Color != Color.Empty)
                {
                    chatSection.Style = string.Format(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2};", node.Color.R, node.Color.G, node.Color.B);
                }
                chatSection.InnerText = node.Text;

                result = chatSection;
            }
            else // need to make a link.
            {
                HtmlElement hrefSection = m_domDoc.CreateElement("a");
                hrefSection.SetAttribute("href", node.LinkUri.ToString());

                if (node.CssClass != null)
                {
                    hrefSection.SetAttribute("className", node.CssClass);
                }
                else if (node.Color != Color.Empty)
                {
                    hrefSection.Style =
                        string.Format(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2};", node.Color.R, node.Color.G, node.Color.B);
                }

                hrefSection.SetAttribute("title", 
                    string.Format(CultureInfo.CurrentCulture, "Link to {0}", node.LinkUri.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")));

                hrefSection.InnerText = node.Text;
                hrefSection.Click += new HtmlElementEventHandler(hrefSection_Click);

                result = hrefSection;
            }

            return result;
        }

        private void hrefSection_Click(object sender, HtmlElementEventArgs e)
        {
            OnLinkClicked(sender, e);
        }

        protected virtual void OnLinkClicked(object sender, HtmlElementEventArgs e)
        {
            HtmlElement elem = e.FromElement;

            System.Diagnostics.Process.Start(elem.GetAttribute("href"));

            e.ReturnValue = false;
        }

        protected virtual HtmlElement CreateLink()
        {
            HtmlElement element = HtmlDomDocument.CreateElement("a");
            element.Click += new HtmlElementEventHandler(hrefSection_Click);

            return element;
        }
    }
}
