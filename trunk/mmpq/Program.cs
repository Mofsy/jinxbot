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
using MBNCSUtil.Data;
using System.IO;

namespace MBNCSUtil.Applications
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { 
                    @"c:\Program Files (x86)\Warcraft III\icons-war3.bni"
                };

            using (MpqArchive archive = MpqServices.OpenArchive(args[0]))
            {
                if (args.Length == 1)
                {
                    string listFile = archive.GetListFile();
                    Console.WriteLine(listFile);
                }
                else
                {
                    for (int i = 1; i < args.Length; i++)
                    {
                        string targetPath = Path.Combine(".", args[i]);
                        targetPath = Path.GetFullPath(targetPath);
                        string targetDirectory = Path.GetDirectoryName(targetPath);
                        if (!Directory.Exists(targetDirectory))
                            Directory.CreateDirectory(targetDirectory);

                        using (MpqFileStream str = archive.OpenFile(args[i]))
                        {
                            byte[] file = new byte[str.Length];
                            str.Read(file, 0, file.Length);
                            File.WriteAllBytes(targetPath, file);
                        }
                    }
                }
                Console.ReadLine();
            }
        }
    }
}
