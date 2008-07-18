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
using System.IO;
using MBNCSUtil.Net;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace GameFileDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a path to which you would like to extract game files:");
            string pathBase = Console.ReadLine();
            if (!Directory.Exists(pathBase))
            {
                Directory.CreateDirectory(pathBase);
            }

            Console.WriteLine("Enter the name of the server to download from:");
            string server = Console.ReadLine();

            BnFtpRequestBase req = new BnFtpVersion1Request("star", "list.txt", null);
            req.Server = server;
            req.LocalFileName = Path.Combine(pathBase, "list.txt");
            req.ExecuteRequest();

            string[] fileList = File.ReadAllLines(req.LocalFileName);

            Dictionary<string, string> files = new Dictionary<string, string>();
            foreach (string file in fileList)
            {
                Match m = Regex.Match(file, @"(?<product>\w{4})-\d\..*\.zip\z");
                if (m.Success)
                {
                    Console.WriteLine("Adding file {0} for product {1}", file, m.Groups["product"].Value);
                    string key = m.Groups["product"].Value;

                    if (files.ContainsKey(key))
                    {
                        Console.WriteLine("Replacing file {0} with file {1} for product {2}", files[key], file, key);
                        files.Remove(key);
                    }
                    files.Add(m.Groups["product"].Value, file);
                }
            }

            foreach (string key in files.Keys)
            {
                Console.WriteLine("Downloading {0}...", key);
                string dirPath = Path.Combine(pathBase, key);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                req = new BnFtpVersion1Request("star", files[key], null);
                req.Server = server;
                req.LocalFileName = Path.Combine(dirPath, files[key]);
                req.ExecuteRequest();

                FastZip fz = new FastZip();
                fz.ExtractZip(req.LocalFileName, pathBase, FastZip.Overwrite.Always, null, string.Empty, string.Empty, true);
            }

            string lockdownBase = Path.Combine(pathBase, "Lockdown");
            if (!Directory.Exists(lockdownBase))
                Directory.CreateDirectory(lockdownBase);

            for (int i = 0; i < 20; i++)
            {
                string num = i.ToString().PadLeft(2, '0');
                req = new BnFtpVersion1Request("star", string.Concat("lockdown-IX86-", num, ".dll"), null);
                req.Server = server;
                req.LocalFileName = Path.Combine(lockdownBase, req.FileName);
                Console.WriteLine("Downloading {0}...", req.LocalFileName);
                req.ExecuteRequest();
            }

            Console.WriteLine("Completed.");
        }
    }
}
