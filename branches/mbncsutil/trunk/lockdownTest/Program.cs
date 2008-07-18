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

namespace MBNCSUtil.Applications
{
    class Program
    {
        static void Main(string[] args)
        {
            string lockdown = @"c:\GameFiles\Lockdown\lockdown-IX86-03.dll";
            string valueStr = "CF15486A6082BC75ABF05906C5BDD00502";
            string file1 = @"c:\GameFiles\STAR\starcraft.exe";
            string file2 = @"c:\GameFiles\STAR\storm.dll";
            string file3 = @"c:\GameFiles\STAR\battle.snp";
            string vImg = @"c:\GameFiles\STAR\star.bin";

            int version = -1;
            int checksum = -1;
            byte[] valStr = ParseHexStr(valueStr);
            byte[] digest = CheckRevision.DoLockdownCheckRevision(valStr, new string[] { file1, file2, file3 }, lockdown, vImg, ref version, ref checksum);

            Console.WriteLine("Checksum: {0:x8}", checksum);
            Console.WriteLine("Version: {0:x8}", version);
            Console.WriteLine("Digest: {0}", DataFormatter.Format(digest));
            Console.ReadLine();
        }

        static byte[] ParseHexStr(string str)
        {
            int byteLength = str.Length / 2;
            if (str.Length % 2 == 1)
            {
                str = string.Concat("0", str);
                byteLength++;
            }

            byte[] bytes = new byte[byteLength];
            for (int i = 0; i < str.Length; i += 2)
            {
                bytes[i / 2] = byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return bytes;
        }
    }
}
