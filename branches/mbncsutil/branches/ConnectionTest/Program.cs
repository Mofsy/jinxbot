/*
MBNCSUtil -- Managed Battle.net Authentication Library
Copyright (C) 2005-2008 by Robert Paveza

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met: 

1.) Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. 
2.) Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution. 
3.) The name of the author may not be used to endorse or promote products derived 
from this software without specific prior written permission. 
	
See LICENSE.TXT that should have accompanied this software for full terms and 
conditions.

*/


using System;
using System.Collections.Generic;
using System.Text;
using MBNCSUtil;
using System.IO;
using MBNCSUtil.Net;

namespace ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string file1 = @"c:\GameFiles\STAR\starcraft.exe";
            string file2 = @"c:\GameFiles\STAR\storm.dll";
            string file3 = @"c:\GameFiles\STAR\battle.snp";
            string vImg = @"c:\GameFiles\STAR\star.bin";

            /*
            string file1 = @"c:\GameFiles\w2bn\warcraft ii bne.exe";
            string file2 = @"c:\GameFiles\w2bn\storm.dll";
            string file3 = @"c:\GameFiles\w2bn\battle.snp";
            string vImg = @"c:\GameFiles\w2bn\w2bn.bin";*/

            ConnectionBase con = new ConnectionBase("uswest.battle.net", 6112);
            DataBuffer buf0x1 = new DataBuffer();
            buf0x1.InsertByte(1);

            if (!con.Connect())
                return;

            BncsPacket pck = new BncsPacket(0x50);
            pck.InsertInt32(0);
            pck.InsertDwordString("IX86");
            pck.InsertDwordString("STAR");
            pck.InsertInt32(0xd1);
/*            pck.InsertInt32(0x4f);*/
            pck.InsertInt32(1033);
            pck.InsertInt32((int)con.LocalEP.Address.Address);
            pck.InsertInt32((int)((DateTime.Now.ToFileTime() - DateTime.UtcNow.ToFileTime()) / 600000000));
            pck.InsertInt32(0);
            pck.InsertDwordString("enUS");
            pck.InsertCString("USA");
            pck.InsertCString("United States");
            con.Send(buf0x1);
            con.Send(pck);

            DataFormatter.WriteToConsole(pck.GetData());

            byte[] res = con.Receive(4);
            int len = BitConverter.ToInt16(res, 2) - 4;
            DataReader reader = new DataReader(con.Receive(len));
            if (res[1] == 0x25)
            {
                Console.WriteLine("Received ping challenge {0:x8}", reader.ReadInt32());
                res = con.Receive(4);
                len = BitConverter.ToInt16(res, 2) - 4;
                Console.WriteLine("Received:");
                res = con.Receive(len);
                DataFormatter.WriteToConsole(res);
                reader = new DataReader(res);
            }

            int logonType = reader.ReadInt32();
            int serverToken = reader.ReadInt32();
            int udpValue = reader.ReadInt32();
            long mpqFiletime = reader.ReadInt64();
            string crevFilename = reader.ReadCString();
            byte[] crevValstr = reader.ReadNullTerminatedByteArray();

            string ldownFilePath = Path.Combine("c:\\gamefiles\\lockdown", crevFilename.ToLower().Replace(".mpq", ".dll"));
            if (!File.Exists(ldownFilePath))
            {
                Console.WriteLine("Specify server to download lockdown file {0}:", crevFilename);
                string srv = Console.ReadLine();
                BnFtpVersion1Request req = new BnFtpVersion1Request("star", crevFilename.Replace(".MPQ", ".dll").Replace(".mpq", ".dll"), null);
                req.Server = srv;
                req.ExecuteRequest();
            }

            int clientToken = new Random().Next();
            int ver = -1;
            int checksum = -1;
            Console.WriteLine("Enter CD key:");
            string key = Console.ReadLine();
            CdKey cdkey = new CdKey(key);

            BncsPacket check = new BncsPacket(0x51);
            check.InsertInt32(clientToken);

            byte[] digest = CheckRevision.DoLockdownCheckRevision(crevValstr, new string[] { file1, file2, file3 }, ldownFilePath, vImg, ref ver, ref checksum);
            check.InsertInt32(ver);
            check.InsertInt32(checksum);
            check.InsertInt32(1);
            check.InsertInt32(0);
            check.InsertInt32(cdkey.Key.Length);
            check.InsertInt32(cdkey.Product);
            check.InsertInt32(cdkey.Value1);
            check.InsertInt32(0);
            check.InsertByteArray(cdkey.GetHash(clientToken, serverToken));

            check.InsertByteArray(digest);
            check.InsertByte(0);
            check.InsertCString("Blah");

            con.Send(check);
            Console.WriteLine("Sending:");
            DataFormatter.WriteToConsole(check.GetData());

            res = con.Receive(4);
            len = BitConverter.ToInt16(res, 2) - 4;
            reader = new DataReader(con.Receive(len));
            Console.WriteLine("Received: {0:x8}", reader.ReadInt32());
            Console.WriteLine("Additional Info: {0}", reader.ReadCString());

            Console.ReadLine();
        }
    }
}
