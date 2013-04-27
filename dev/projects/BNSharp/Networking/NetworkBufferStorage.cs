using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    [DebuggerDisplay("{_buffers.Count} buffer(s) available, {_rootBuffer.Length / 1024}KB total buffer size")]
    internal class NetworkBufferStorage
    {
        private byte[] _rootBuffer;
        private object _syncObject = new object();
        private int _size, _count;

        private Stack<NetworkBuffer> _buffers;

        internal NetworkBufferStorage(int perBufferSize, int buffersToAlloc)
        {
            //Debug.Assert(perBufferSize > 0);
            //Debug.Assert(buffersToAlloc > 0);

            if (perBufferSize * buffersToAlloc >= 1048576)
                throw new ArgumentException("Cannot allocate a 1mb or larger buffer storage.");

            _size = perBufferSize;
            _count = buffersToAlloc;

            _rootBuffer = new byte[perBufferSize * buffersToAlloc];
            _buffers = new Stack<NetworkBuffer>(buffersToAlloc);

            // allocate all the network buffers
            for (int i = 0; i < buffersToAlloc; i++)
            {
                NetworkBuffer buffer = new NetworkBuffer(_rootBuffer, i * _size, _size, this);
                _buffers.Push(buffer);
            }

#if DEBUG
            ClearOnRelease = true;
#endif
        }

        public bool ClearOnRelease
        {
            get;
            set;
        }

        public NetworkBuffer Acquire()
        {
            lock (_syncObject)
            {
                Debug.Assert(_buffers.Count > 0);

                return _buffers.Pop();
            }
        }

        public async void Release(NetworkBuffer buffer)
        {
            DebugVerifyBufferIsNotAlreadyInList(buffer);

            if (ClearOnRelease)
                await buffer.Clear();

            Debug.Assert(buffer.Parent == this);

            lock (_syncObject)
            {
                _buffers.Push(buffer);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void DebugVerifyBufferIsNotAlreadyInList(NetworkBuffer buffer)
        {
            Debug.Assert(!_buffers.Contains(buffer));
        }
    }
}
