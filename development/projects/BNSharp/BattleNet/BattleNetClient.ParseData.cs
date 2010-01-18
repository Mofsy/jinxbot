using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BNSharp.BattleNet
{
    partial class BattleNetClient
    {
        /// <summary>
        /// Contains data from the receive thread that needs to be used by the parsing thread.
        /// </summary>
        /// <remarks>
        /// <para>To maximize efficiency, this class does not protect any of its members through properties.  It should only be used when
        /// implementing a custom handler for one or more messages.</para>
        /// </remarks>
        [CLSCompliant(false)]
        public class ParseData
        {
            internal ParseData(byte packetID, ushort len, byte[] data)
            {
                PacketID = (BncsPacketId)packetID;
                Length = len;
                Data = data;
            }

            /// <summary>
            /// Contains the packet ID associated with this data.
            /// </summary>
            public BncsPacketId PacketID;
            /// <summary>
            /// Contains the length specified by Battle.net for this data, minus the four bytes used in the header.
            /// </summary>
            public ushort Length;
            /// <summary>
            /// Contains the literal data sent from Battle.net.
            /// </summary>
            public byte[] Data;
        }

        partial void FreeArgumentResources(BaseEventArgs e)
        {
            if (e == null)
                return;

            ParseData data = e.EventData;
            if (data != null && data.Data != null)
            {
                if (data.Data.Length == BattleNetClientResources.IncomingBufferPool.BufferLength)
                {
                    BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
                }

                data.Data = null;
            }
        }
    }
}
