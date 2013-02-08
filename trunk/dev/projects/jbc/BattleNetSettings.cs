using BNSharp.BattleNet;
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
            CdKey1 = Console.ReadLine();

            Client = ClassicProduct.StarcraftRetail;
            VersionByte = 0xd3;
            GameExe = File.OpenRead(@"F:\Game Files\STAR\Starcraft.exe");
            GameFile2 = File.OpenRead(@"F:\Game Files\STAR\Storm.dll");
            GameFile3 = File.OpenRead(@"F:\Game Files\STAR\Battle.snp");
            ImageFile = File.OpenRead(@"F:\Game Files\STAR\STAR.bin");

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

        public string CdKey1
        {
            get;
            set;
        }

        public string CdKey2
        {
            get;
            set;
        }

        public System.IO.Stream GameExe
        {
            get;
            set;
        }

        public System.IO.Stream GameFile2
        {
            get;
            set;
        }

        public System.IO.Stream GameFile3
        {
            get;
            set;
        }

        public System.IO.Stream ImageFile
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
