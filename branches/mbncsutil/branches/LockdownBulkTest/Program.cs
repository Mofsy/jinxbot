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
using System.Xml;
using MBNCSUtil;
using System.IO;

namespace LockdownBulkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage:  LockdownBulkText {w2bn|star}");
                return;
            }
            string prod = args[0];
            string xml = prod + ".xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(xml);

            XmlNode node = doc.DocumentElement;
            // node is Keys
            //System.Diagnostics.Debugger.Break();
            List<LDSet> items = new List<LDSet>();
            foreach (XmlNode tr in node.ChildNodes)
            {
                LDSet set = new LDSet(tr);
                if (set.mpq.ToUpper().StartsWith("LOCKDOWN"))
                    items.Add(set);
            }
            string basePath = @"c:\gamefiles\lockdown";

            string[] star = new string[] { 
                @"c:\GameFiles\STAR\starcraft.exe",
                @"c:\GameFiles\STAR\storm.dll",
                @"c:\GameFiles\STAR\battle.snp",
                @"c:\GameFiles\STAR\star.bin"
            };
            string[] w2bn = new string[] {
                @"c:\GameFiles\w2bn\warcraft ii bne.exe",
                @"c:\GameFiles\w2bn\storm.dll",
                @"c:\GameFiles\w2bn\battle.snp",
                @"c:\GameFiles\w2bn\w2bn.bin"
            };

            string file1, file2, file3, vImg;
            if (prod == "w2bn")
            {
                file1 = w2bn[0];
                file2 = w2bn[1];
                file3 = w2bn[2];
                vImg = w2bn[3];
            }
            else
            {
                file1 = star[0];
                file2 = star[1];
                file3 = star[2];
                vImg = star[3];
            }

            int failures = 0;
            List<Result> failed = new List<Result>();
            for (int i = 0; i < items.Count; i++)
            {
                int ver = -1;
                int csum = -1;
                byte[] digestResult = CheckRevision.DoLockdownCheckRevision(items[i].RequestValue, new string[] { file1, file2, file3 }, Path.Combine(basePath, items[i].mpq.Replace(".mpq", ".dll")), vImg, ref ver, ref csum);
                if (ver.ToString("x8").ToUpper() == items[i].exever.ToUpper().PadLeft(8, '0') &&
                    csum.ToString("x8").ToUpper() == items[i].checksum.ToUpper().PadLeft(8, '0') &&
                    FmtBin(digestResult).ToUpper() == items[i].digest)
                {
                    Console.WriteLine("{0}:\t\tValid", i);
                }
                else
                {
                    failures++;
                    Console.WriteLine("{0}: Invalid, {1:x8}, {2:x8}, {3}", items[i].id, ver, csum, FmtBin(digestResult));
                    Console.WriteLine("\tExpected: EXE ver {0}, received {1:X8}", items[i].exever.PadLeft(8, '0'), ver);
                    Console.WriteLine("\tExpected: Checksum {0}, received {1:X8}", items[i].checksum.PadLeft(8, '0'), csum);
                    Console.WriteLine("\tExpected: Digest {0}", items[i].digest);
                    Console.WriteLine("\tReceived: Digest {0}", FmtBin(digestResult));

                    failed.Add(new Result(items[i], digestResult, csum, ver));
                }
            }

            XmlTextWriter xtw = new XmlTextWriter(string.Format("{0}-results.xml", prod), Encoding.UTF8);
            xtw.Indentation = 4;
            xtw.IndentChar = ' ';
            xtw.Formatting = Formatting.Indented;

            xtw.WriteStartDocument();
            xtw.WriteStartElement("processing-results");
            xtw.WriteAttributeString("total-processed", items.Count.ToString());
            xtw.WriteAttributeString("failures", failures.ToString());
            xtw.WriteAttributeString("success-rate", ((double)((items.Count - failures) * 100) / (double)items.Count).ToString());

            foreach (Result failure in failed)
            {
                failure.WriteXml(xtw);
            }

            xtw.WriteEndElement();
            xtw.WriteEndDocument();
            xtw.Flush();

            Console.ReadLine();
        }

        internal static string FmtBin(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        static bool FastCmp(byte[] val1, byte[] val2)
        {
            int[] a = new int[val1.Length / 4];
            int[] b = new int[val2.Length / 4];
            Buffer.BlockCopy(val1, 0, a, 0, val1.Length);
            Buffer.BlockCopy(val2, 0, b, 0, val2.Length);
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }

    class Result
    {
        public LDSet set;
        public byte[] digest;
        public int checksum, exeVer;

        public Result(LDSet expected, byte[] calculatedDigest, int calculatedChecksum, int calculatedExever)
        {
            set = expected;
            digest = calculatedDigest;
            checksum = calculatedChecksum;
            exeVer = calculatedExever;
        }

        public void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("failure");
            writer.WriteElementString("mpq", set.mpq);
            writer.WriteElementString("value-string", set.request);
            writer.WriteElementString("expected-exe-version", set.exever.ToUpper().PadLeft(8, '0'));
            writer.WriteElementString("obtained-exe-version", exeVer.ToString("X8"));
            writer.WriteElementString("expected-checksum", set.checksum.ToUpper().PadLeft(8, '0'));
            writer.WriteElementString("obtained-checksum", checksum.ToString("X8"));
            writer.WriteElementString("expected-digest", set.digest.ToUpper());
            writer.WriteElementString("obtained-digest", Program.FmtBin(digest).ToUpper());
            writer.WriteEndElement();
        }
    }

    class LDSet
    {
        public LDSet(XmlNode tr)
        {
            XmlNode id = tr.FirstChild;
            XmlNode prod = id.NextSibling;
            XmlNode mpq = prod.NextSibling;
            XmlNode req = mpq.NextSibling;
            XmlNode exe = req.NextSibling;
            XmlNode cks = exe.NextSibling;
            XmlNode res = cks.NextSibling;

            this.id = id.InnerText;
            product = prod.InnerText;
            this.mpq = mpq.InnerText;
            request = req.InnerText;
            exever = exe.InnerText;
            checksum = cks.InnerText;
            digest = res.InnerText;
        }

        public string product, mpq, request, exever, checksum, digest, id;
        public byte[] DigestValue
        {
            get
            {
                return ParseHexStr(digest);
            }
        }

        public byte[] RequestValue
        {
            get
            {
                return ParseHexStr(request);
            }
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
