using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Views;
using Microsoft.WindowsAPICodePack.Taskbar;
using JinxBot.Plugins.UI;
using System.Windows.Forms;
using JinxBot.Controls.Docking;
using System.Drawing;
using System.Drawing.Imaging;

namespace JinxBot.Windows7
{
    internal static class ThumbnailPreviewManager
    {
        private static Form m_parent;
        private static TabbedThumbnailManager m_manager;
        private static Dictionary<DockableDocument, Bitmap> m_previews = new Dictionary<DockableDocument, Bitmap>();

        public static void Initialize(IMainWindow mainWindow)
        {
            Form form = (mainWindow as Form);
            m_parent = form;

            m_manager = TaskbarManager.Instance.TabbedThumbnail;
        }

        public static void AddThumbnail(DockableDocument doc)
        {
            TabbedThumbnail thumb = new TabbedThumbnail(m_parent.Handle, doc);
            thumb.TabbedThumbnailActivated += (s, e) => doc.Show();
            m_manager.AddThumbnailPreview(thumb);
            Bitmap bmp = new Bitmap(doc.Width, doc.Height, PixelFormat.Format32bppArgb);
            //doc.DrawToBitmap(bmp, doc.Bounds);
            doc.Controls[0].DrawToBitmap(bmp, doc.Controls[0].Bounds);
            m_previews[doc] = bmp;
            thumb.TabbedThumbnailBitmapRequested += 
                (s, e) => 
                {
                    doc.Show();
                    Bitmap newBmp = new Bitmap(doc.Width, doc.Height, PixelFormat.Format32bppArgb);
                    //doc.DrawToBitmap(bmp, doc.Bounds);
                    doc.Controls[0].DrawToBitmap(newBmp, doc.Controls[0].Bounds);
                    e.SetImage(newBmp);
                    e.Handled = true;
                };
        }

        public static void RemoveThumbnail(DockableDocument doc)
        {
            m_manager.RemoveThumbnailPreview(doc);
            m_previews.Remove(doc);
        }
    }
}
