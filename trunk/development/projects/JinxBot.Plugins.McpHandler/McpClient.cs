using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp;
using BNSharp.MBNCSUtil;

namespace JinxBot.Plugins.McpHandler
{
    // it's necessary to override ConnectionBase in order to acquire non-protected access to Send().
    internal class McpClient : ConnectionBase
    {
        internal McpClient(string server, int port)
            : base(server, port)
        {

        }

        internal void Send(DataBuffer packet)
        {
            Send(packet.UnderlyingBuffer, 0, packet.Count);
            BattleNetClientResources.OutgoingBufferPool.FreeBuffer(packet.UnderlyingBuffer);
        }
    }
}
