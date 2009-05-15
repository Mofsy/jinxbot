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
using MBNCSUtil.Net;

namespace MBNCSUtil.Applications
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { "SEXP_IX86_1xx_1161.mpq" };
            BnFtpVersion1Request req = new BnFtpVersion1Request("STAR", args[0], null);
            if (args.Length > 1)
                req.Server = args[1];

            req.FilePartDownloaded += new DownloadStatusEventHandler(req_FilePartDownloaded);
            req.ExecuteRequest();
        }

        static void req_FilePartDownloaded(object sender, DownloadStatusEventArgs e)
        {
            Console.WriteLine("Downloaded {0}/{1} ({2:#.00}%)...", e.DownloadStatus, e.FileLength, (((double)e.DownloadStatus * 100.0) / (double)e.FileLength));
        }
    }
}
