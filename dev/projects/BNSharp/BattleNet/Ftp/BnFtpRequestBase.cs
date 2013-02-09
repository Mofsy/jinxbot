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
using BNSharp.BattleNet;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Ftp
{
    /// <summary>
    /// Represents a generic Battle.net FTP (BnFTP) request.
    /// </summary>
    /// <remarks>
    /// <para>The specific Battle.net FTP protocol is implemented by the 
    /// <see cref="BnFtpVersion1Request">BnFtpVersion1Request</see> and 
    /// <see cref="BnFtpVersion2Request">BnFtpVersion2Request</see> classes, which 
    /// have their uses based on which client is being emulated.  For Warcraft 3 and
    /// The Frozen Throne, <b>BnFtpVersion2Request</b> should be used; otherwise, 
    /// <b>BnFtpVersion1Request</b> should be used.</para>
    /// </remarks>
    public abstract class BnFtpRequestBase
    {
        private string _fileName, _localFile;
        private ClassicProduct _product;
        private Gateway _server;
        private int _size;
        private DateTime? _time;

        /// <summary>
        /// Creates a new generic Battle.net FTP request.
        /// </summary>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <param name="product">The client being emulated.</param>
        /// <param name="fileTime">The timestamp of the file's last write in UTC.
        /// You may specify <b>null</b> (<b>Nothing</b> in Visual Basic) if the 
        /// time is unavailable.</param>
        /// <remarks>
        /// <para>Valid emulation clients include:
        /// <list type="bullet">
        ///     <item>Starcraft Retail</item>
        ///     <item>Starcraft: Brood War</item>
        ///     <item>Warcraft II: Battle.net Edition</item>
        ///     <item>Diablo II Retail</item>
        ///     <item>Diablo II: Lord of Destruction</item>
        ///     <item>Warcraft III: The Reign of Chaos</item>
        ///     <item>Warcraft III: The Frozen Throne</item>
        /// </list>
        /// </para>
        /// </remarks>
        protected BnFtpRequestBase(string fileName, ClassicProduct product, DateTime? fileTime)
        {
            if (!product.CanConnect)
                throw new ArgumentException("The requested product cannot be used for BnFtp requests.", "product");

            _fileName = fileName;
            if (fileName.IndexOf('\\') != -1)
            {
                _fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            }
            _product = product;

            _time = fileTime;

            this.LocalFileName = fileName;
        }

        /// <summary>
        /// Gets the Product utilized by this request.
        /// </summary>
        public virtual ClassicProduct Product { get { return _product; } }

        #region IBnFtpRequest Members

        /// <summary>
        /// Gets or sets the local path of the file.
        /// </summary>
        /// <remarks>
        /// <para>This property must be set before the <see cref="ExecuteRequest">ExecuteRequest</see> method is 
        /// called.  It can be changed in subsequent calls to download the same file to multiple locations; however,
        /// changing this property will not affect files that have already been downloaded.</para>
        /// </remarks>
        public string LocalFileName
        {
            get
            {
                return _localFile;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!value.Contains("\\"))
                {
                    value = Path.GetFullPath(string.Concat(".\\", value));
                }
                string tmpPath = Path.GetDirectoryName(value);
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }

                _localFile = value;
            }
        }

        /// <summary>
        /// Gets the name of the filed being requested.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets (and in derived classes, sets) the size of the file.
        /// </summary>
        /// <remarks>
        /// <para>This property is only valid after 
        /// <see cref="ExecuteRequest">ExecuteRequest</see> has been called.</para>
        /// </remarks>
        public int FileSize
        {
            get { return _size; }
            protected set
            {
                _size = value;
            }
        }

        /// <summary>
        /// Gets the local file's last-write time, if it was specified.  If it was not specified, this property
        /// returns <b>null</b> (<b>Nothing</b> in Visual Basic).
        /// </summary>
        public DateTime? FileTime { get { return _time; } }

        /// <summary>
        /// Gets or sets the server from which this request should download.
        /// </summary>
        /// <remarks>
        /// <para>The default gateway is <see cref="Gateway.USEast"/>.</para>
        /// </remarks>
        public Gateway Gateway
        {
            get
            {
                return _server;
            }
            set
            {
                if (value == null)
                    value = Gateway.USEast;

                _server = value;
            }
        }

        /// <summary>
        /// Executes the BnFTP request, downloading the file to where <see cref="LocalFileName">LocalFileName</see>
        /// specifies, and closes the connection.
        /// </summary>
        /// <remarks>
        /// <para>By default, <c>LocalFileName</c> is the same name as the remote file, which will cause the file
        /// to be saved in the local application path.  The desired location of the file must be set before 
        /// <b>ExecuteRequest</b> is called.</para>
        /// </remarks>
        /// <exception cref="IOException">Thrown if the local file cannot be written.</exception>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown if the remote host closes the connection prematurely.</exception>
        public abstract Task ExecuteRequest();

        #endregion
    }
}
