using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.MBNCSUtil.Data;
using System.Drawing;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;
using JinxBot.Configuration;

namespace JinxBot.Windows7
{
    internal static class JumpListIconManager
    {
        public static void CreateJumpListImages(string bniFilePath)
        {
            string jlIconsPath = Path.Combine(Path.GetDirectoryName(bniFilePath), "JumpListIcons");
            if (!Directory.Exists(jlIconsPath))
                Directory.CreateDirectory(jlIconsPath);

            using (BniFileParser bni = new BniFileParser(bniFilePath))
            {
                foreach (BniIcon icon in bni.AllIcons)
                {
                    if (icon.SoftwareProductCodes.Length == 0)
                        continue;

                    string filename = icon.SoftwareProductCodes[0] + ".ico";

                    using (Bitmap result = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    using (Graphics g = Graphics.FromImage(result))
                    {
                        g.DrawImage(icon.Image, new Rectangle(0, 6, 32, 20));

                        using (Icon ico = Icon.FromHandle(result.GetHicon()))
                        using (FileStream fs = new FileStream(Path.Combine(jlIconsPath, filename), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                        {
                            ico.Save(fs);
                        }
                    }
                }
            }
        }

        public static IconReference CreateIconForClient(string clientName)
        {
            string path = Path.Combine(JinxBotConfiguration.ApplicationDataPath, "JumpListIcons");
            switch (clientName.ToLower())
            {
                case "d2dv":
                case "d2xp":
                case "star":
                case "sexp":
                case "w2bn":
                case "war3":
                case "w3xp":
                case "jstr":
                    path = Path.Combine(path, clientName + ".ico");
                    return new IconReference(path, 0);
                default:
                    path = Path.Combine(path, "chat.ico");
                    return new IconReference(path, 0);
            }
        }
    }
}
