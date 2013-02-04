using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Views.Chat;
using JinxBot.Plugins.UI;
using System.Drawing;
using BNSharp.BattleNet;

namespace JinxBot.Views
{
    internal static class UiIconProvider
    {
        private static IIconProvider m_icons = new WebIconProvider();

        public static Image GetIconForClient(string client)
        {
            switch (client.ToLower())
            {
                case "d2dv":
                case "d2xp":
                case "star":
                case "sexp":
                case "w2bn":
                case "war3":
                case "w3xp":
                case "jstr":
                    Image img = m_icons.GetImageFor(Product.GetByProductCode(client));
                    return img;
                default:
                    Image img2 = m_icons.GetImageFor(Product.ChatClient);
                    return img2;
            }
        }
    }
}
