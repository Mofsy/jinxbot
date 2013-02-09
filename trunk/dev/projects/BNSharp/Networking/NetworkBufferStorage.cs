﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    internal class NetworkBufferStorage
    {
        private byte[] _rootBuffer;
        private object _syncObject = new object();
        private int _size, _count;

        private Stack<NetworkBuffer> _buffers;

        internal NetworkBufferStorage(int perBufferSize, int buffersToAlloc)
        {
            Debug.Assert(perBufferSize > 0);
            Debug.Assert(buffersToAlloc > 0);

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

        public void Release(NetworkBuffer buffer)
        {
            if (ClearOnRelease)
                buffer.Clear();

            Debug.Assert(buffer.Parent == this);

            lock (_syncObject)
            {
                _buffers.Push(buffer);
            }
        }
    }
}