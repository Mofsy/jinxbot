using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using JinxBot.Plugins.UI;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

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
        public override Inline Render(ChatNode node)
        {
            IIconProvider provider = ProfileResourceProvider.GetForClient(null).Icons;

            ImageChatNode icn = node as ImageChatNode;
            if (icn != null)
            {
                InlineUIContainer result = new InlineUIContainer();
                Image img = new Image();
                img.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap((icn.Image as System.Drawing.Bitmap).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                img.ToolTip = icn.Text;
                img.Width = provider.IconSize.Width;
                img.Height = provider.IconSize.Height;

                result.Child = img;

                if (icn.LinkUri != null)
                {
                    Hyperlink container = new Hyperlink(result);
                    container.NavigateUri = node.LinkUri;
                    container.ToolTip = string.Format(CultureInfo.CurrentUICulture, "Link to {0}", node.LinkUri);

                    return container;
                }

                return result;
            }
            else
            {
                return base.Render(node);
            }
        }
    }
}
