using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.Net;

namespace BNSharp
{
    /// <summary>
    /// Provides global resources preallocated for performance.
    /// </summary>
    public static class BattleNetClientResources
    {
        private static List<BattleNetClient> s_activeClients = new List<BattleNetClient>();

        private static BufferPool s_incoming = new BufferPool("Incoming Packets", 1024, 5);
        private static BufferPool s_outgoing = new BufferPool("Outgoing Packets", 768, 5) { ClearOnFree = true };

        private const int INCOMING_BUFFERS_PER_CLIENT = 25;
        private const int OUTGOING_BUFFERS_PER_CLIENT = 10;

        /// <summary>
        /// Gets the <see>BufferPool</see> used for incoming packets.
        /// </summary>
        public static BufferPool IncomingBufferPool
        {
            get { return s_incoming; }
        }

        /// <summary>
        /// Gets the <see>BufferPool</see> used for outgoing packets.
        /// </summary>
        public static BufferPool OutgoingBufferPool
        {
            get { return s_outgoing; }
        }

        /// <summary>
        /// Registers a client connection, tracking it and increasing the available buffer pool.
        /// </summary>
        /// <param name="client">The client connection that is being registered.</param>
        public static void RegisterClient(BattleNetClient client)
        {
            s_activeClients.Add(client);
            s_incoming.IncreaseBufferCount(INCOMING_BUFFERS_PER_CLIENT);
            s_outgoing.IncreaseBufferCount(OUTGOING_BUFFERS_PER_CLIENT);
        }

        /// <summary>
        /// Unregisters a client connection, halting tracking and decreasing the available buffer pool.
        /// </summary>
        /// <param name="client">The client connection being unregistered.</param>
        public static void UnregisterClient(BattleNetClient client)
        {
            s_activeClients.Remove(client);
            s_incoming.DecreaseBufferCount(INCOMING_BUFFERS_PER_CLIENT);
            s_outgoing.DecreaseBufferCount(OUTGOING_BUFFERS_PER_CLIENT);
        }
    }
}
