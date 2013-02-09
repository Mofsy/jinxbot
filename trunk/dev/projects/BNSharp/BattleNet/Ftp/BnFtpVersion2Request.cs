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
using BNSharp.BattleNet.Core;
using System.Threading.Tasks;
using BNSharp.Networking;

namespace BNSharp.BattleNet.Ftp
{
    /// <summary>
    /// Represents a Battle.net FTP (BnFTP) file transfer request for Version 2 products.
    /// </summary>
    /// <remarks>
    /// <para>This class is only valid for Warcraft III: The Reign of Chaos and Warcraft III: The Frozen Throne.
    /// For Starcraft Retail, Starcraft: Brood War, Diablo II Retail, Diablo II: Lord of Destruction, and Warcraft
    /// II: Battle.net Edition clients, use the <see cref="BnFtpVersion1Request">BnFtpVersion1Request</see>
    /// class.</para>
    /// </remarks>
    public class BnFtpVersion2Request : BnFtpRequestBase
    {
        private int _adId;
        private string _adExt;
        private CdKey _key;
        private bool _ad;

        /// <summary>
        /// Creates a standard Version 2 Battle.net FTP request.
        /// </summary>
        /// <param name="product">The product being emulated by this request.</param>
        /// <param name="fileName">The full or relative path to the file as it is to be stored on the local 
        /// machine.  The name portion of the file must be the filename being requested from the service.</param>
        /// <param name="fileTime">The last-write time of the file.  If the file is not available, this parameter
        /// can be <b>null</b> (<b>Nothing</b> in Visual Basic).</param>
        /// <param name="cdKey">The CD key of the client being emulated.</param>
        public BnFtpVersion2Request(string fileName, ClassicProduct product, DateTime fileTime, CdKey cdKey)
            : base(fileName, product, fileTime)
        {
            string prod = Product.ProductCode;
            if (prod != "WAR3" && prod != "W3XP")
                throw new ArgumentOutOfRangeException("product", product, "Only Warcraft III clients are valid for Battle.net version 2 FTP requests.");

            if (!cdKey.IsValid)
                throw new ArgumentException("The specified CD key is not valid.", "cdKey");
            _key = cdKey;
        }

        /// <summary>
        /// Creates a Version 2 Battle.net FTP request specifically for banner ad downloads.
        /// </summary>
        /// <param name="product">The product being emulated by this request.</param>
        /// <param name="fileName">The full or relative path to the file as it is to be stored on the local 
        /// machine.  The name portion of the file must be the filename being requested from the service.</param>
        /// <param name="fileTime">The last-write time of the file.  If the file is not available, this parameter
        /// can be <b>null</b> (<b>Nothing</b> in Visual Basic).</param>
        /// <param name="cdKey">The CD key of the client being emulated.</param>
        /// <param name="adId">The banner ID provided by Battle.net's ad notice message.</param>
        /// <param name="adFileExtension">The banner filename extension provided by Battle.net's ad notice message.</param>
        /// <remarks>
        /// <para>Although it is not specifically required to download banner ads, it is recommended for forward-compatibility
        /// with the Battle.net protocol that this constructor is used.</para>
        /// </remarks>
        public BnFtpVersion2Request(string fileName, ClassicProduct product, DateTime fileTime, CdKey cdKey, int adId, string adFileExtension)
            : this(fileName, product, fileTime, cdKey)
        {
            m_ad = true;
            m_adId = adId;
            m_adExt = adFileExtension;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "time")]
        public override async Task ExecuteRequest()
        {
            using (AsyncConnectionBase connection = new AsyncConnectionBase(Gateway.ServerHost, Gateway.ServerPort, 0, 0))
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(FileName);

                DataBuffer buf = new DataBuffer();
                buf.InsertInt16(20); // Length
                buf.InsertInt16(0x0200); // Protocol version
                buf.InsertDwordString("IX86");
                buf.InsertDwordString(Product);
                if (m_ad)
                {
                    buf.InsertInt32(m_adId);
                    buf.InsertDwordString(m_adExt);
                }
                else
                {
                    buf.InsertInt64(0);
                }

                bool connected = await connection.ConnectAsync();
                if (!connected)
                    throw new IOException("Battle.net refused the connection.");

                await connection.SendAsync(new byte[] { 2 });

                byte[] outgoingData = buf.UnderlyingStream.ToArray();
                outgoingData = await connection.SendAsync(outgoingData);

                if (outgoingData == null)
                    throw new IOException("Unable to send request to Battle.net.");

                byte[] incomingData = new byte[4];
                incomingData = await connection.ReceiveAsync(incomingData, 0, 4);
                if (incomingData == null)
                    throw new IOException("Battle.net rejected the request.");

                int serverToken = BitConverter.ToInt32(incomingData, 0);

                buf = new DataBuffer();
                buf.InsertInt32(0); // No resuming
                if (FileTime.HasValue)
                    buf.InsertInt64(FileTime.Value.ToFileTimeUtc());
                else
                    buf.InsertInt64(0);
                int clientToken = new Random().Next();
                buf.InsertInt32(clientToken);

                buf.InsertInt32(_key.Key.Length);
                buf.InsertInt32(_key.Product);
                buf.InsertInt32(_key.Value1);
                buf.InsertInt32(0);
                buf.InsertByteArray(_key.GetHash(clientToken, serverToken));
                buf.InsertByteArray(fileNameBytes);
                buf.InsertByte(0);

                outgoingData = buf.UnderlyingStream.ToArray();
                outgoingData = await connection.SendAsync(outgoingData);
                if (outgoingData == null)
                    throw new IOException("Could not send file request to Battle.net.");

                incomingData = new byte[8];
                incomingData = await connection.ReceiveAsync(incomingData, 0, 8);
                if (incomingData == null)
                    throw new IOException("Battle.net rejected the file request.");

                int remainingHeaderSize = BitConverter.ToInt32(incomingData, 0) - 8;
                int fileSize = BitConverter.ToInt32(incomingData, 4);
                this.FileSize = fileSize;
                incomingData = new byte[remainingHeaderSize];
                incomingData = await connection.ReceiveAsync(incomingData, 0, remainingHeaderSize);
                if (incomingData == null)
                    throw new IOException("Battle.net did not send a complete file header.");

                DataReader reader = new DataReader(incomingData);
                reader.Seek(8); // banner id / extension
                long fileTime = reader.ReadInt64();
                this.FileTime = DateTime.FromFileTimeUtc(fileTime);
                string name = reader.ReadCString();
                if (string.Compare(name, FileName, StringComparison.OrdinalIgnoreCase) != 0 || FileSize == 0)
                {
                    throw new FileNotFoundException("The specified file was not found by Battle.net.");
                }

                incomingData = new byte[fileSize];
                incomingData = await connection.ReceiveAsync(incomingData, 0, fileSize);
                if (incomingData == null)
                    throw new IOException("Battle.net did not send the file data.");

                File.WriteAllBytes(LocalFileName, fileData);
                DateTime time = DateTime.FromFileTimeUtc(fileTime);
                File.SetLastWriteTimeUtc(LocalFileName, time);
            }
        }
    }
}