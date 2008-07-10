using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using JinxBot.Controls;
using System.IO;
using System.Drawing.Imaging;

namespace JinxBot.Views.Chat
{
    /// <summary>
    /// Represents a chat node that renders an image.
    /// </summary>
    [ChatNodeRenderer(typeof(ImageChatNodeRenderer))]
    public class ImageChatNode : ChatNode
    {
        private string m_imgName;
        
        /// <summary>
        /// Creates a new <see>ImageChatNode</see> with the specified parameters.
        /// </summary>
        /// <param name="imageName">The name of the image.</param>
        /// <param name="img">The image to render.</param>
        /// <param name="altText">The alternate/title text.  This parameter is optional.</param>
        public ImageChatNode(string imageName, Image img, string altText) 
            : base(altText ?? string.Empty, Color.Tan)
        {
            if (imageName == null)
                throw new ArgumentNullException("imageName");
            if (img == null)
                throw new ArgumentNullException("img");

            m_imgName = imageName;

            lock (img)
            {
                if (!ImageChatNodeProtocol.HasRegistered(imageName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (EncoderParameters ep = new EncoderParameters(1))
                    {
                        ImageCodecInfo ici = (from enc in ImageCodecInfo.GetImageEncoders()
                                              where enc.MimeType == "image/jpeg"
                                              select enc).FirstOrDefault();
                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        img.Save(ms, ici, ep);

                        byte[] memory = new byte[ms.Length];
                        Buffer.BlockCopy(ms.GetBuffer(), 0, memory, 0, memory.Length);
                        ImageChatNodeProtocol.RegisterImage(imageName, memory);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        public string ImageName
        {
            get { return m_imgName; }
        }
    }
}
