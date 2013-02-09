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


using BNSharp.BattleNet.Core;
using BNSharp.Networking;
using System;
using System.IO;

namespace BNSharp.BattleNet.Core
{
    /// <summary>
    /// Completes a <see cref="DataBuffer">DataBuffer</see> implementation with the additional
    /// data used by the BNCS protocol.
    /// </summary>
    /// <remarks>
    /// <para>When using this class with a Stream, the BncsReader only takes the next packet's data
    /// off of the stream.  An ideal example of this would be when using a <see cref="System.Net.Sockets.NetworkStream">NetworkSteam</see>
    /// to connect to Battle.net.  Incidentally, this constructor and method will block execution until new data has arrived.  Therefore,
    /// if your main receiving loop is going to use these methods, it should be on a background worker loop.</para>
    /// </remarks>
    public class BncsReader : DataReader
    {
        private int _len = 0;

        /// <summary>
        /// Gets the length of the data.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this reader is not yet initialized.</exception>
        public override int Length
        {
            get { return _len; }
        }

        /// <summary>
        /// Gets or sets the ID of the packet as it was specified when it was created.
        /// </summary>
        public BncsPacketId PacketID
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new data reader with the specified byte data.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <exception cref="ArgumentNullException">Thrown if <b>data</b> is 
        /// <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public BncsReader(NetworkBuffer buffer)
            : base(buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (this.ReadByte() != 0xff)
                throw new InvalidDataException("The buffer was invalid.");
            PacketID = (BncsPacketId)this.ReadByte();
            _len = this.ReadUInt16();
            if (_len > 8192)
                throw new InvalidDataException("The buffer was invalid.");
        }

        /// <summary>
        /// Gets a hex representation of this data.
        /// </summary>
        /// <returns>A string representing this buffer's contents in hexadecimal notation.</returns>
        public override string ToString()
        {
            return DataFormatter.Format(this.UnderlyingBuffer.UnderlyingBuffer, 0, this.Length);
        }

    }
}
