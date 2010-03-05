using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.McpHandler
{
    internal class McpParseData
    {
        internal McpParseData(byte packetID, ushort len, byte[] data)
        {
            PacketID = (McpPacketId)packetID;
            Length = len;
            Data = data;
        }

        /// <summary>
        /// Contains the packet ID associated with this data.
        /// </summary>
        public McpPacketId PacketID;
        /// <summary>
        /// Contains the length specified by Battle.net for this data, minus the four bytes used in the header.
        /// </summary>
        public ushort Length;
        /// <summary>
        /// Contains the literal data sent from Battle.net.
        /// </summary>
        public byte[] Data;
    }
}
