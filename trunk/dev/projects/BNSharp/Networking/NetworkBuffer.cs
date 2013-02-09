using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    /// <summary>
    /// Represents an opaque data buffer used to store network traffic.  
    /// </summary>
    public class NetworkBuffer
    {
        private byte[] _underlyingBuffer;
        private int _bufferStart, _bufferLength;
        private NetworkBufferStorage _parent;

        internal NetworkBuffer(byte[] storageBuffer, int startsAt, int length, NetworkBufferStorage parent)
        {
            Debug.Assert(storageBuffer != null);
            Debug.Assert(startsAt < storageBuffer.Length);
            Debug.Assert(startsAt % 8 == 0);
            Debug.Assert(length % 8 == 0);

            _parent = parent;
            _underlyingBuffer = storageBuffer;
            _bufferStart = startsAt;
            _bufferLength = length;
        }

        internal NetworkBufferStorage Parent
        {
            get { return _parent; }
        }

        internal byte[] UnderlyingBuffer
        {
            get { return _underlyingBuffer; } 
        }

        internal int StartingPosition
        {
            get { return _bufferStart; }
        }

        /// <summary>
        /// Gets the maximum length of the buffer.
        /// </summary>
        public int Length
        {
            get { return _bufferLength; }
        }

        /// <summary>
        /// Zeroes out the memory in the buffer.
        /// </summary>
        public unsafe void Clear()
        {
            fixed (byte* ptr = &_underlyingBuffer[_bufferStart])
            {
                int count = _bufferLength / 8;
                long* tgt = (long*)ptr;
                for (int i = 0; i < count; i++)
                {
                    *tgt++ = 0;
                }
            }
        }
    }
}
