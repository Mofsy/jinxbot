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
using System.IO;
using System.Text;
using System.Diagnostics;

namespace BNSharp.Networking
{
    /// <summary>
    /// Operates as a buffered data reader for network and file input.
    /// </summary>
    /// <remarks>
    /// <para>This class does not write data in any manner; for writing or sending data, 
    /// use the <see cref="DataBuffer">DataBuffer</see> or derived classes.</para>
    /// <para>This class always uses little-endian byte ordering.</para>
    /// </remarks>
    public class DataReader
    {
        private NetworkBuffer _buffer;
        private byte[] _data;
        private int _index, _baseIndex, _baseSize;

        /// <summary>
        /// Gets a reference to the underlying buffer.
        /// </summary>
        public NetworkBuffer UnderlyingBuffer
        {
            get
            {
                return _buffer;
            }
        }

        internal byte[] CoreUnderlyingData
        {
            get
            {
                return _data;
            }
        }

        #region ctors
        /// <summary>
        /// Creates a new data reader with the specified byte array.
        /// </summary>
        /// <param name="buffer">The source of the data to read.</param>
        public DataReader(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            _data = buffer;
            _baseSize = buffer.Length;
        }

        /// <summary>
        /// Creates a new data reader with the specified network buffer.
        /// </summary>
        /// <param name="buffer">The source of the buffer to read.</param>
        public DataReader(NetworkBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            _buffer = buffer;
            _data = buffer.UnderlyingBuffer;
            _baseIndex = buffer.StartingPosition;
            _baseSize = buffer.Length;
        }
        #endregion

        #region DataReader Members
        /// <summary>
        /// Reads a boolean value from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method interprets a 32-bit value from the stream as false if it is zero and true if it is nonzero.</para>
        /// </remarks>
        /// <returns>The next boolean value from the data stream.</returns>
        public bool ReadBoolean()
        {
            return BitConverter.ToInt32(_data, _index + _baseIndex) != 0;
        }

        /// <summary>
        /// Reads a byte value from the data stream.
        /// </summary>
        /// <returns>The next byte from the data stream.</returns>
        public byte ReadByte()
        {
            return _data[_baseIndex + _index++];
        }

        /// <summary>
        /// Reads a byte array from the data stream.
        /// </summary>
        /// <param name="expectedItems">The number of bytes to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> bytes from the stream.</returns>
        public byte[] ReadByteArray(int expectedItems)
        {
            byte[] data = new byte[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems);
            _index += expectedItems;
            return data;
        }

        /// <summary>
        /// Reads a null-terminated byte array from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>The return value includes the null terminator.</para>
        /// </remarks>
        /// <returns>The next byte array in the stream, terminated by a value of 0.</returns>
        public byte[] ReadNullTerminatedByteArray()
        {
            int i = _index + _baseIndex;

            while ((i < _data.Length) && (_data[i] != 0))
                i++;

            byte[] bytes = new byte[i - (_index + _baseIndex)];
            Buffer.BlockCopy(_data, _index + _baseIndex, bytes, 0, bytes.Length);

            _index += bytes.Length;

            return bytes;
        }

        /// <summary>
        /// Reads a signed 16-bit value from the data stream.
        /// </summary>
        /// <returns>The next 16-bit value from the data stream.</returns>
        public short ReadInt16()
        {
            short s = BitConverter.ToInt16(_data, _index + _baseIndex);
            _index += 2;
            return s;
        }

        /// <summary>
        /// Reads an array of signed 16-bit values from the data stream.
        /// </summary>
        /// <param name="expectedItems">The number of 16-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 16-bit values from the stream.</returns>
        public short[] ReadInt16Array(int expectedItems)
        {
            short[] data = new short[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 2);
            _index += (expectedItems * 2);
            return data;
        }

        /// <summary>
        /// Reads an unsigned 16-bit value from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <returns>The next 16-bit value from the data stream.</returns>
        [CLSCompliant(false)]
        public ushort ReadUInt16()
        {
            ushort s = BitConverter.ToUInt16(_data, _index + _baseIndex);
            _index += 2;
            return s;
        }

        /// <summary>
        /// Reads an array of unsigned 16-bit values from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <param name="expectedItems">The number of 16-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 16-bit values from the stream.</returns>
        [CLSCompliant(false)]
        public ushort[] ReadUInt16Array(int expectedItems)
        {
            ushort[] data = new ushort[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 2);
            _index += (expectedItems * 2);
            return data;
        }

        /// <summary>
        /// Reads a signed 32-bit value from the data stream.
        /// </summary>
        /// <returns>The next 32-bit value from the data stream.</returns>
        public int ReadInt32()
        {
            int i = BitConverter.ToInt32(_data, _index + _baseIndex);
            _index += 4;
            return i;
        }

        /// <summary>
        /// Reads an array of signed 32-bit values from the data stream.
        /// </summary>
        /// <param name="expectedItems">The number of 32-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 32-bit values from the stream.</returns>
        public int[] ReadInt32Array(int expectedItems)
        {
            int[] data = new int[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 4);
            _index += (expectedItems * 4);
            return data;
        }

        /// <summary>
        /// Reads an unsigned 32-bit value from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <returns>The next 32-bit value from the data stream.</returns>
        [CLSCompliant(false)]
        public uint ReadUInt32()
        {
            uint i = BitConverter.ToUInt32(_data, _index + _baseIndex);
            _index += 4;
            return i;
        }

        /// <summary>
        /// Reads an array of signed 32-bit values from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <param name="expectedItems">The number of 32-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 32-bit values from the stream.</returns>
        [CLSCompliant(false)]
        public uint[] ReadUInt32Array(int expectedItems)
        {
            uint[] data = new uint[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 4);
            _index += (expectedItems * 4);
            return data;
        }

        /// <summary>
        /// Reads a signed 64-bit value from the data stream.
        /// </summary>
        /// <returns>The next 64-bit value from the data stream.</returns>
        public long ReadInt64()
        {
            long l = BitConverter.ToInt64(_data, _index + _baseIndex);
            _index += 8;
            return l;
        }

        /// <summary>
        /// Reads an array of signed 64-bit values from the data stream.
        /// </summary>
        /// <param name="expectedItems">The number of 64-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 64-bit values from the stream.</returns>
        public long[] ReadInt64Array(int expectedItems)
        {
            long[] data = new long[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 8);
            _index += (expectedItems * 8);
            return data;
        }

        /// <summary>
        /// Reads an unsigned 64-bit value from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <returns>The next 64-bit value from the data stream.</returns>
        [CLSCompliant(false)]
        public ulong ReadUInt64()
        {
            ulong l = BitConverter.ToUInt64(_data, _index + _baseIndex);
            _index += 8;
            return l;
        }

        /// <summary>
        /// Reads an array of unsigned 64-bit values from the data stream.
        /// </summary>
        /// <remarks>
        /// <para>This method is not CLS-compliant.</para>
        /// </remarks>
        /// <param name="expectedItems">The number of 64-bit values to read from the stream.</param>
        /// <returns>The next <i>expectedItems</i> 64-bit values from the stream.</returns>
        [CLSCompliant(false)]
        public ulong[] ReadUInt64Array(int expectedItems)
        {
            ulong[] data = new ulong[expectedItems];
            Buffer.BlockCopy(_data, _index + _baseIndex, data, 0, expectedItems * 8);
            _index += (expectedItems * 8);
            return data;
        }

        /// <summary>
        /// Reads the next byte in the stream but does not consume it.
        /// </summary>
        /// <returns>A byte value (0-255) if the call succeeded, or else -1 if reading past the end of the stream.</returns>
        public int Peek()
        {
            if ((_index + _baseIndex) >= _data.Length)
                return -1;
            return _data[_index + _baseIndex];
        }

        /// <summary>
        /// Peeks at the next possible four-byte string with the specified byte padding without advancing the index.
        /// </summary>
        /// <param name="padding">The byte used to pad the string to total four bytes.</param>
        /// <returns>The next 4-byte string, reversed, from the stream.</returns>
        public string PeekDwordString(byte padding)
        {
            int length = _data.Length - (_index + _baseIndex);
            if (length > 4)
                length = 4;

            byte[] b = new byte[length];
            int idx0 = -1;
            for (int i = (_index + _baseIndex), j = length - 1; i < ((_index + _baseIndex) + length) && j >= 0; i++, j--)
            {
                b[j] = _data[i];
            }

            int startIndex = 0;
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == padding)
                    startIndex++;
                else
                    break;
            }

            int endIndex = b.Length - 1;
            for (int i = b.Length - 1; i >= 0; i--)
            {
                if (b[i] == padding)
                    endIndex--;
                else
                    break;
            }

            int len = (endIndex - startIndex) + 1;
            if (len < 0) len = 0;
            else if (len > b.Length) len = b.Length;

            string result = Encoding.ASCII.GetString(b, startIndex, len);
            return result;
        }

        /// <summary>
        /// Reads the next possible four-byte string with the specified byte padding.
        /// </summary>
        /// <param name="padding">The byte used to pad the string to total four bytes.</param>
        /// <returns>The next 4-byte string, reversed, from the stream.</returns>
        public string ReadDwordString(byte padding)
        {
            string str = PeekDwordString(padding);
            _index += 4;
            return str;
        }

        /// <summary>
        /// Reads the next C-style ASCII null-terminated string from the stream.
        /// </summary>
        /// <returns>The next C-style string.</returns>
        public string ReadCString()
        {
            return ReadCString(Encoding.ASCII);
        }

        /// <summary>
        /// Reads the next C-style null-terminated string from the stream.
        /// </summary>
        /// <param name="enc">The encoding used for the string.</param>
        /// <returns>The next C-style string encoded with the specified encoding.</returns>
        public string ReadCString(Encoding enc)
        {
            return ReadTerminatedString('\0', enc);
        }

        /// <summary>
        /// Reads the next pascal-style ASCII string from the stream.
        /// </summary>
        /// <returns>The next pascal-style string.</returns>
        public string ReadPascalString()
        {
            return ReadPascalString(Encoding.ASCII);
        }

        /// <summary>
        /// Reads the next pascal-style string from the stream.
        /// </summary>
        /// <param name="enc">The encoding used for the string.</param>
        /// <returns>The next pascal-style string encoded with the specified encoding.</returns>
        public string ReadPascalString(Encoding enc)
        {
            int len = ReadByte();
            string s = enc.GetString(_data, _index + _baseIndex, len);
            _index += enc.GetByteCount(s);
            return s;
        }

        /// <summary>
        /// Reads the next wide-pascal-style string from the stream.
        /// </summary>
        /// <returns>The next wide-pascal-style string.</returns>
        public string ReadWidePascalString()
        {
            return ReadWidePascalString(Encoding.ASCII);
        }

        /// <summary>
        /// Reads the next wide-pascal-style string from the stream.
        /// </summary>
        /// <param name="enc">The encoding used for the string.</param>
        /// <returns>The next wide-pascal-style string encoded with the specified encoding.</returns>
        public string ReadWidePascalString(Encoding enc)
        {
            int len = ReadInt16();
            string s = enc.GetString(_data, _index + _baseIndex, len);
            _index += enc.GetByteCount(s);
            return s;
        }

        /// <summary>
        /// Gets the length of the data.
        /// </summary>
        public virtual int Length
        {
            get
            {
                return _baseSize;
            }
        }

        #endregion

        /// <summary>
        /// Returns the next variable-length string with the specified terminator character.
        /// </summary>
        /// <param name="Terminator">The terminator that should indicate the end of the string.</param>
        /// <param name="enc">The encoding to use to read the string.</param>
        /// <returns>A variable-length string with no NULL (0) characters nor the terminator character.</returns>
        public string ReadTerminatedString(char Terminator, Encoding enc)
        {
            int i = _index + _baseIndex;
            if (enc == Encoding.Unicode || enc == Encoding.BigEndianUnicode)
            {
                while ((i < _data.Length) && ((i + 1 < _data.Length) && (BitConverter.ToChar(_data, i) != Terminator)))
                    i++;
            }
            else
            {
                while ((i < _data.Length) && (_data[i] != (Terminator & 0xff)))
                    i++;
            }

            string s = enc.GetString(_data, _index + _baseIndex, i - _index - _baseIndex);
            _index = (++i - _baseIndex);

            return s;
        }

        /// <summary>
        /// Checks to see whether the offset from the current position lies within the stream and, if so, advances to
        /// that position relative to the current location.
        /// </summary>
        /// <param name="offset">The number of bytes beyond the current position to advance to.</param>
        /// <returns><b>True</b> if the position lies within the stream and the cursor was advanced; otherwise <b>false</b>.</returns>
        public bool Seek(int offset)
        {
            bool fOk = false;
            if (this._index + offset <= _baseSize)
            {
                _index += offset;
                fOk = true;
            }
            return fOk;
        }

        /// <summary>
        /// Gets the current position within the stream.
        /// </summary>
        public int Position
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets a hex representation of this buffer.
        /// </summary>
        /// <returns>A string representing this buffer's contents in hexadecimal notation.</returns>
        public override string ToString()
        {
            return DataFormatter.Format(_data, _baseIndex, _baseSize);
        }
    }
}
