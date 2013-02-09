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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    /// <summary>
    /// Operates as a data buffer for network and file output.
    /// </summary>
    /// <remarks>
    /// <para>This class does not read data in any manner; for reading or receiving data, 
    /// use the <see cref="DataReader">DataReader</see> or derived classes.</para>
    /// <para>This class always uses little-endian byte ordering.</para>
    /// <para>This class no longer supports operator overloading in C# via the binary addition (+) 
    /// operator.</para>
    /// </remarks>
    public class DataBuffer
    {
        private NetworkBuffer _target;
        private MemoryStream _ms;
        private byte[] _buffer;
        private int _len;

        /// <summary>
        /// Creates a new variable-sized DataBuffer.
        /// </summary>
        public DataBuffer() 
        {
            _ms = new MemoryStream();
        }

        /// <summary>
        /// Creates a new <b>DataBuffer</b> backed by a NetworkBuffer.
        /// </summary>
        public DataBuffer(NetworkBuffer targetBuffer)
        {
            if (targetBuffer == null)
                throw new ArgumentNullException("targetBuffer");

            _target = targetBuffer;
            _buffer = targetBuffer.UnderlyingBuffer;
            _ms = new MemoryStream(_buffer, targetBuffer.StartingPosition, targetBuffer.Length, true);
        }

        #region Insert methods
        /// <summary>
        /// Inserts the specified Boolean value into the buffer.
        /// </summary>
        /// <param name="b">The value to insert.</param>
        /// <remarks>
        /// This method inserts a 32-bit value of 1 or 0 based on the value of the 
        /// parameter: 1 if the value is <b>true</b>, or otherwise 0.
        /// </remarks>
        public void InsertBoolean(bool b)
        {
            if (b)
                InsertInt32(1);
            else
                InsertInt32(0);
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="b">The value to insert.</param>
        public void InsertByte(byte b)
        {
            lock (this)
            {
                _ms.WriteByte(b);
                _len++;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <remarks>This method is not CLS-compliant.</remarks>
        /// <param name="b">The value to insert.</param>
        [CLSCompliant(false)]
        public void InsertSByte(sbyte b)
        {
            lock (this)
            {
                _ms.WriteByte(unchecked((byte)b));
                _len++;
            }
        }

        /// <summary>
        /// Inserts the specified byte array into the buffer.
        /// </summary>
        /// <param name="b">The value to insert.</param>
        public void InsertByteArray(byte[] b)
        {
            lock (this)
            {
                _ms.Write(b, 0, b.Length);
                _len += b.Length;
            }
        }

        /// <summary>
        /// Inserts the specified byte array into the buffer.
        /// </summary>
        /// <param name="b">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertSByteArray(sbyte[] b)
        {
            byte[] unsigned = new byte[b.Length];
            Buffer.BlockCopy(b, 0, unsigned, 0, b.Length);
            lock (this)
            {
                _ms.Write(unsigned, 0, b.Length);
                _len += b.Length;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="s">The value to insert.</param>
        public void InsertInt16(short s)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(s), 0, 2);
                _len += 2;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <remarks>This method is not CLS-compliant.</remarks>
        /// <param name="s">The value to insert.</param>
        [CLSCompliant(false)]
        public void InsertUInt16(ushort s)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(s), 0, 2);
                _len += 2;
            }
        }

        /// <summary>
        /// Inserts the specified 16-bit data array into the buffer.
        /// </summary>
        /// <param name="s">The value to insert.</param>
        public void InsertInt16Array(short[] s)
        {
            byte[] result = new byte[s.Length * 2];
            Buffer.BlockCopy(s, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }

        /// <summary>
        /// Inserts the specified 16-bit data array into the buffer.
        /// </summary>
        /// <param name="s">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertUInt16Array(ushort[] s)
        {
            byte[] result = new byte[s.Length * 2];
            Buffer.BlockCopy(s, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="i">The value to insert.</param>
        public void InsertInt32(int i)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(i), 0, 4);
                _len += 4;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="i">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertUInt32(uint i)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(i), 0, 4);
                _len += 4;
            }
        }

        /// <summary>
        /// Inserts the specified 32-bit data array into the buffer.
        /// </summary>
        /// <param name="i">The value to insert.</param>
        public void InsertInt32Array(int[] i)
        {
            byte[] result = new byte[i.Length * 4];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }

        /// <summary>
        /// Inserts the specified 32-bit data array into the buffer.
        /// </summary>
        /// <param name="i">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertUInt32Array(uint[] i)
        {
            byte[] result = new byte[i.Length * 4];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="l">The value to insert.</param>
        public void InsertInt64(long l)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(l), 0, 8);
                _len += 8;
            }
        }

        /// <summary>
        /// Inserts the specified value into the buffer.
        /// </summary>
        /// <param name="l">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertUInt64(ulong l)
        {
            lock (this)
            {
                _ms.Write(BitConverter.GetBytes(l), 0, 8);
                _len += 8;
            }
        }

        /// <summary>
        /// Inserts the specified 64-bit data array into the buffer.
        /// </summary>
        /// <param name="l">The value to insert.</param>
        public void InsertInt64Array(long[] l)
        {
            byte[] result = new byte[l.Length * 8];
            Buffer.BlockCopy(l, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }

        /// <summary>
        /// Inserts the specified 64-bit data array into the buffer.
        /// </summary>
        /// <param name="l">The value to insert.</param>
        /// <remarks>This method is not CLS-compliant.</remarks>
        [CLSCompliant(false)]
        public void InsertUInt64Array(ulong[] l)
        {
            byte[] result = new byte[l.Length * 8];
            Buffer.BlockCopy(l, 0, result, 0, result.Length);

            lock (this)
            {
                _ms.Write(result, 0, result.Length);
                _len += result.Length;
            }
        }
        #endregion

        #region Strings
        /// <summary>
        /// Inserts the specified value into the buffer as a C-style null-terminated 
        /// ASCII string.
        /// </summary>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string terminated by a single null (0) byte.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void InsertCString(string str)
        {
            InsertCString(str, Encoding.ASCII);
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a C-style null-terminated
        /// string using the specified encoding.
        /// </summary>
        /// <param name="enc">The byte encoding to use.</param>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string terminated by a null character.  For 
        /// 8-bit character encodings such as ASCII, this null character is also 8 bits.
        /// For 16-bit character encodings such as Unicode, this null character is also
        /// 16 bits.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void InsertCString(string str, Encoding enc)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (enc == null)
                throw new ArgumentNullException("enc");

            InsertByteArray(enc.GetBytes(str));
            InsertByteArray(new byte[enc.GetByteCount(new char[] { '\0' })]);
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a pascal-style ASCII string.
        /// </summary>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string prefixed by the total number of characters
        /// in the string.  At most a string may be 255 characters.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 255 characters.</exception>
        public void InsertPascalString(string str)
        {
            InsertPascalString(str, Encoding.ASCII);
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a pascal-style string using 
        /// the specified encoding.
        /// </summary>
        /// <param name="enc">The encoding to use.</param>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string prefixed by the total number of characters
        /// in the string.  At most a string may be 255 characters.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 255 characters.</exception>
        public void InsertPascalString(string str, Encoding enc)
        {
            if (str.Length > 255)
                throw new ArgumentException("String length was too long; max length 255.", "str");

            InsertByte((byte)(str.Length & 0xff));
            InsertByteArray(enc.GetBytes(str));
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a wide-pascal-style ASCII
        /// string.
        /// </summary>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string prefixed by the total number of characters
        /// in the string.  At most a string may be 65,535 characters.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 65,535 characters.</exception>
        public void InsertWidePascalString(string str)
        {
            InsertWidePascalString(str, Encoding.ASCII);
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a wide-pascal-style string
        /// using the specified encoding.
        /// </summary>
        /// <param name="enc">The encoding to use.</param>
        /// <param name="str">The string value to insert.</param>
        /// <remarks>
        /// <para>This method inserts a string prefixed by the total number of characters
        /// in the string.  At most a string may be 65,535 characters.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either <c>str</c> or <c>enc</c> were <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 65,535 characters.</exception>
        public void InsertWidePascalString(string str, Encoding enc)
        {
            if (str.Length > 65535)
                throw new ArgumentException("String length was too long; max length 65535.", "str");
            InsertUInt16((ushort)(str.Length & 0xffff));
            InsertByteArray(enc.GetBytes(str));
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a 32-bit-style string using 
        /// null bytes as the default padding.
        /// </summary>
        /// <remarks>
        /// <para>This method inserts a string with the maximum length of 4 into the 
        /// buffer, reversed.  This mimics the C-style declarations of 4-character 
        /// integer literals:</para>
        /// <code>unsigned long int star_product = 'STAR';</code>
        /// <para>which results in <c>RATS</c> being in memory.</para>
        /// </remarks>
        /// <param name="str">The string which may be at most 4 characters.</param>
        /// <exception cref="ArgumentNullException">The <c>str</c> parameter was <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 4 characters.</exception>
        public void InsertDwordString(string str)
        {
            InsertDwordString(str, 0);
        }

        /// <summary>
        /// Inserts the specified value into the buffer as a 32-bit-style string using 
        /// the specified byte as padding.
        /// </summary>
        /// <remarks>
        /// <para>This method inserts a string with the maximum length of 4 into the 
        /// buffer, reversed.  This mimics the C-style declarations of 4-character 
        /// integer literals:</para>
        /// <code>unsigned long int star_product = 'STAR';</code>
        /// <para>which results in <c>RATS</c> being in memory.</para>
        /// </remarks>
        /// <param name="str">The string which may be at most 4 characters.</param>
        /// <param name="padding">The byte which shall be used to pad the string if it is less than 4 characters long.</param>
        /// <exception cref="ArgumentNullException">The <c>str</c> parameter was <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The length of <c>str</c> was too great; maximum string length is 4 characters.</exception>
        public void InsertDwordString(string str, byte padding)
        {
            if (str.Length > 4)
                throw new ArgumentException("String length was too long; max length 4.", "str");

            lock (this)
            {
                if (str.Length < 4)
                {
                    int numNulls = 4 - str.Length;
                    for (int i = 0; i < numNulls; i++)
                        InsertByte(padding);
                }
                byte[] bar = Encoding.ASCII.GetBytes(str);
                for (int i = bar.Length - 1; i >= 0; i--)
                    InsertByte(bar[i]);
            }
        }
        #endregion

        /// <summary>
        /// Gets the contents of the underlying buffer.
        /// </summary>
        public NetworkBuffer UnderlyingBuffer
        {
            get { return _target; }
        }

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public MemoryStream UnderlyingStream
        {
            get { return _ms; }
        }

        /// <summary>
        /// Writes the data currently contained in the buffer to the specified stream.
        /// </summary>
        /// <param name="str">The stream to which to write the data.</param>
        /// <returns>The number of bytes written to the stream.</returns>
        public virtual async Task<int> WriteToOutputStreamAsync(Stream str)
        {
            int count = _len;
            await str.WriteAsync(_target.UnderlyingBuffer, 0, count);
            return count;
        }

        public virtual async Task<int> SendAsync(AsyncConnectionBase connection)
        {
            int len = _len;
            await connection.SendAsync(_target, len);
            return len;
        }

        /// <summary>
        /// Gets the length of data in the buffer.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _len;
            }
        }

        /// <summary>
        /// Gets a hex representation of this buffer.
        /// </summary>
        /// <returns>A string representing this buffer's contents in hexadecimal notation.</returns>
        public override string ToString()
        {
            return DataFormatter.Format(_buffer, 0, Count);
        }

    }
}
