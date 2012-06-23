using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BNSharp.BattleNet;

namespace JinxBot.LoginService.Models
{
    public class FileData
    {
        public string GameExe { get; private set; }
        public string StormDll { get; private set; }
        public string GameDll { get; private set; }
        public string ImageData { get; private set; }

        private static readonly FileData Starcraft = new FileData { GameExe = "starcraft.exe", StormDll = "storm.dll", GameDll = "battle.snp", ImageData = "STAR.bin", };
        private static readonly FileData Warcraft2 = new FileData { GameExe = "Warcraft II BNE.exe", StormDll = "storm.dll", GameDll = "battle.snp", ImageData = "W2BN.bin", };
        private static readonly FileData Diablo2 = new FileData { GameExe = "Game.exe", StormDll = "Bnclient.dll", GameDll = "D2Client.dll", };
        private static readonly FileData Warcraft3 = new FileData { GameExe = "War3.exe", StormDll = "storm.dll", GameDll = "Game.dll", };

        public static FileData GetForProduct(Product product)
        {
            switch (product.ProductCode)
            {
                case "STAR":
                case "SEXP":
                case "SSHR":
                case "JSTR":
                    return Starcraft;

                case "W2BN":
                    return Warcraft2;

                case "D2DV":
                case "D2XP":
                    return Diablo2;

                case "WAR3":
                case "W3XP":
                    return Warcraft3;

                default:
                    return null;
            }
        }
    }
}