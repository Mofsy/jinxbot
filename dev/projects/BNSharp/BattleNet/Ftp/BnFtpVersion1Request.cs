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
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;
using BNSharp;
using BNSharp.Networking;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Ftp
{
    /// <summary>
    /// Represents a Battle.net FTP (BnFTP) file transfer request for Version 1 products.
    /// </summary>
    /// <remarks>
    /// <para>This class is only valid for Starcraft Retail, Starcraft: Brood War, Diablo II Retail, 
    /// Diablo II: Lord of Destruction, and Warcraft II: Battle.net Edition clients.  For Warcraft III: The Reign
    /// of Chaos and Warcraft III: The Frozen Throne, use the <see cref="BnFtpVersion2Request">BnFtpVersion2Request</see>
    /// class.</para>
    /// </remarks>
    public class BnFtpVersion1Request : BnFtpRequestBase
    {
        private string _adExt;
        private int _adId;
        private bool _ad;

        // 33 + fileName.Length

        /// <summary>
        /// Creates a standard Version 1 Battle.net FTP request.
        /// </summary>
        /// <param name="productId">The four-character identifier for the product being emulated by this request.</param>
        /// <param name="fileName">The full or relative path to the file as it is to be stored on the local 
        /// machine.  The name portion of the file must be the filename being requested from the service.</param>
        /// <param name="fileTime">The last-write time of the file.  If the file is not available, this parameter
        /// can be <b>null</b> (<b>Nothing</b> in Visual Basic).</param>
        public BnFtpVersion1Request(ClassicProduct product, string fileName, DateTime? fileTime)
            : base(fileName, product, fileTime)
        {
            string prod = this.Product.ProductCode;

            if (prod == "WAR3" || prod == "W3XP")
                throw new ArgumentOutOfRangeException("product", product, "Cannot make a BnFtp version 1 request with Warcraft III: The Reign of Chaos or Warcraft III: The Frozen Throne.");
        }

        /// <summary>
        /// Creates a Version 1 Battle.net FTP request specifically for banner ad downloads.
        /// </summary>
        /// <param name="productId">The four-character identifier for the product being emulated by this request.</param>
        /// <param name="fileName">The full or relative path to the file as it is to be stored on the local 
        /// machine.  The name portion of the file must be the filename being requested from the service.</param>
        /// <param name="fileTime">The last-write time of the file.  If the file is not available, this parameter
        /// can be <b>null</b> (<b>Nothing</b> in Visual Basic).</param>
        /// <param name="adBannerId">The banner ID provided by Battle.net's ad notice message.</param>
        /// <param name="adBannerExtension">The banner filename extension provided by Battle.net's ad notice message.</param>
        /// <remarks>
        /// <para>Although it is not specifically required to download banner ads, it is recommended for forward-compatibility
        /// with the Battle.net protocol that this constructor is used.</para>
        /// </remarks>
        public BnFtpVersion1Request(ClassicProduct product, string fileName, DateTime fileTime, int adBannerId, string adBannerExtension)
            : this(product, fileName, fileTime)
        {
            _adExt = adBannerExtension;
            _adId = adBannerId;
            _ad = true;
        }

        /// <summary>
        /// Executes the BnFTP request, downloading the file to where <see cref="BnFtpRequestBase.LocalFileName">LocalFileName</see>
        /// specifies, and closes the connection.
        /// </summary>
        /// <remarks>
        /// <para>By default, <c>LocalFileName</c> is the same name as the remote file, which will cause the file
        /// to be saved in the local application path.  The desired location of the file must be set before 
        /// <b>ExecuteRequest</b> is called.</para>
        /// </remarks>
        /// <exception cref="IOException">Thrown if the local file cannot be written.</exception>
        /// <exception cref="SocketException">Thrown if the remote host closes the connection prematurely.</exception>
        public override async Task ExecuteRequest()
        {
            using (AsyncConnectionBase connection = new AsyncConnectionBase(Gateway.ServerHost, Gateway.ServerPort, 0, 0))
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(FileName);

                DataBuffer buffer = new DataBuffer();
                buffer.InsertInt16((short)(33 + fileNameBytes.Length));
                buffer.InsertInt16(0x0100);
                buffer.InsertDwordString("IX86");
                buffer.InsertDwordString(Product.ProductCode);
                if (_ad)
                {
                    buffer.InsertInt32(_adId);
                    buffer.InsertDwordString(_adExt);
                }
                else
                {
                    buffer.InsertInt64(0);
                }
                // currently resuming is not supported
                buffer.InsertInt32(0);
                if (FileTime.HasValue)
                {
                    buffer.InsertInt64(FileTime.Value.ToFileTimeUtc());
                }
                else
                {
                    buffer.InsertInt64(0);
                }
                buffer.InsertByteArray(fileNameBytes);
                buffer.InsertByte(0);


                bool connected = await connection.ConnectAsync();
                if (!connected)
                    throw new IOException("Battle.net refused the connection to FTP.");

                await connection.SendAsync(new byte[] { 2 });

                byte[] byteData = buffer.UnderlyingStream.ToArray();
                await connection.SendAsync(byteData);

                byte[] header = new byte[8];
                header = await connection.ReceiveAsync(header, 0, 8);

                if (header == null)
                    throw new IOException("Battle.net did not respond to the FTP request.");

                DataReader headerReader = new DataReader(header);
                ushort headerLength = headerReader.ReadUInt16();
                headerReader.Seek(2);
                int fileSize = headerReader.ReadInt32();
                this.FileSize = fileSize;

                byte[] remainingHeader = new byte[headerLength - 8];
                remainingHeader = await connection.ReceiveAsync(remainingHeader, 0, headerLength - 8);
                if (remainingHeader == null)
                    throw new IOException("Battle.net did not send the complete header.");

                headerReader = new DataReader(remainingHeader);
                headerReader.Seek(8);
                long fileTime = headerReader.ReadInt64();
                string name = headerReader.ReadCString();
                if (string.Compare(name, FileName, StringComparison.OrdinalIgnoreCase) != 0 || FileSize == 0)
                {
                    throw new FileNotFoundException("The specified file was not found by Battle.net.");
                }
                Debug.WriteLine(fileSize, "File Size");

                byte[] fileData = new byte[fileSize];
                fileData = await connection.ReceiveAsync(fileData, 0, fileSize);
                if (fileData == null)
                    throw new IOException("Battle.net did not send the file data.");

                File.WriteAllBytes(LocalFileName, fileData);
                DateTime time = DateTime.FromFileTimeUtc(fileTime);
                File.SetLastWriteTimeUtc(LocalFileName, time);
            }
        }
    }
}
