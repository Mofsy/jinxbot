using BNSharp.BattleNet;
using BNSharp.BattleNet.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbc
{
    internal class BattleNetSettings : IBattleNetSettings
    {
        public BattleNetSettings()
        {
            Console.Write("Username: ");
            Username = Console.ReadLine();
            Console.Write("Password: ");
            Password = Console.ReadLine();
            Console.Write("CD Key: ");
            CdKey1 = new CdKey(Console.ReadLine());

            Client = ClassicProduct.StarcraftRetail;
            VersionByte = 0xd3;
            GameExe = @"C:\GameFiles\STAR\Starcraft.exe";
            GameFile2 = @"C:\GameFiles\STAR\Storm.dll";
            GameFile3 = @"C:\GameFiles\STAR\Battle.snp";
            ImageFile = @"C:\GameFiles\STAR\STAR.bin";

            Gateway = Gateway.USEast;
            CdKeyOwner = Username;
            PingMethod = PingKind.ReplyBeforeVersioning;
        }

        public ClassicProduct Client
        {
            get;
            set;
        }

        public int VersionByte
        {
            get;
            set;
        }

        public CdKey CdKey1
        {
            get;
            set;
        }

        public CdKey CdKey2
        {
            get;
            set;
        }

        public string GameExe
        {
            get;
            set;
        }

        public string GameFile2
        {
            get;
            set;
        }

        public string GameFile3
        {
            get;
            set;
        }

        public string ImageFile
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public Gateway Gateway
        {
            get;
            set;
        }

        public string CdKeyOwner
        {
            get;
            set;
        }

        public PingKind PingMethod
        {
            get;
            set;
        }

        public string HomeChannel
        {
            get;
            set;
        }
    }
}
