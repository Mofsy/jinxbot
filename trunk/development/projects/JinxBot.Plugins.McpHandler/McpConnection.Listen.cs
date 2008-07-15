using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp;
using System.Net;
using System.Threading;
using System.Diagnostics;
using BNSharp.MBNCSUtil;


namespace JinxBot.Plugins.McpHandler
{
    partial class McpConnection
    {
        private McpClient m_connection;
        private Queue<McpParseData> m_packetQueue;
        private EventWaitHandle m_parseWait = new EventWaitHandle(false, EventResetMode.AutoReset);

        private void ConnectRealm()
        {
            m_connection = new McpClient(m_ip.ToString(), m_port);
            if (!m_connection.Connect())
            {
                // TODO: Raise an error event.
            }
            else
            {
                McpPacket pck = new McpPacket((byte)McpPacketId.Startup);
                pck.InsertInt32(m_mcpCookie);
                pck.InsertInt32(m_mcpStatus);
                pck.InsertByteArray(m_mcpChunk1);
                pck.InsertByteArray(m_mcpChunk2);
                pck.InsertCString(m_uniqueBnetName);

                Send(pck);
            }
        }

        private void Listen()
        {
            byte[] header = new byte[3];
            try
            {
                while (m_connection.IsConnected)
                {
                    byte[] hdr = m_connection.Receive(header, 0, 3);
                    if (hdr == null) return; // disconnected.
                    byte[] result = null;
                    ushort length = BitConverter.ToUInt16(hdr, 0);
                    if (length > 3)
                    {
                        if (length > BattleNetClientResources.IncomingBufferPool.BufferLength)
                            throw new ProtocolViolationException("Battle.net realm server message was too long to process.");

                        byte[] data = BattleNetClientResources.IncomingBufferPool.GetBuffer();
                        result = m_connection.Receive(data, 0, unchecked((ushort)(length - 3)));
                        if (result == null) return; // disconnected.
                    }
                    else if (length == 3)
                    {
                        length = unchecked((ushort)(length - 3));
                        result = new byte[0];
                    }
                    else
                    {
                        throw new ProtocolViolationException("Battle.net specified an invalid packet length.");
                    }
                    McpParseData parseData = new McpParseData(hdr[2], length, result);
                    m_packetQueue.Enqueue(parseData);
                    m_parseWait.Set();
                }
            }
            catch (ThreadAbortException)
            {
                // exit the thread gracefully.
            }
        }

        private void Parse()
        {
            try
            {
                while (m_connection.IsConnected)
                {
                    m_parseWait.Reset();

                    while (m_packetQueue.Count == 0)
                    {
                        m_parseWait.WaitOne();
                    }

                    McpParseData data = m_packetQueue.Dequeue();

                    switch (data.PacketID)
                    {
                        case McpPacketId.Startup:
                            break;
                        case McpPacketId.CharacterCreate:
                            break;
                        case McpPacketId.CreateGame:
                            break;
                        case McpPacketId.JoinGame:
                            break;
                        case McpPacketId.GameList:
                            break;
                        case McpPacketId.GameInfo:
                            break;
                        case McpPacketId.CharacterLogon:
                            break;
                        case McpPacketId.CharacterDelete:
                            break;
                        case McpPacketId.RequestLadderData:
                            break;
                        case McpPacketId.MessageOfTheDay:
                            break;
                        case McpPacketId.CreateQueue:
                            break;
                        case McpPacketId.CharacterList:
                            break;
                        case McpPacketId.CharacterUpgrade:
                            break;
                        case McpPacketId.CharacterList2:
                            break;
                        default:
                            Trace.WriteLine(data.PacketID, "Unhandled/unknown realm server packet");
                            break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // exit the thread gracefully.
            }
        }

        private void Send(DataBuffer buf)
        {
            m_connection.Send(buf);
        }
    }
}
