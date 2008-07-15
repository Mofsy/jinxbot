using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Plugins;
using BNSharp.MBNCSUtil;
using System.Net;
using PD = BNSharp.Net.BattleNetClient.ParseData;
using BNSharp;
using BNSharp.Net;

namespace JinxBot.Plugins.McpHandler
{
    partial class McpConnection
    {
        private int m_mcpCookie, m_mcpStatus;
        private byte[] m_mcpChunk1, m_mcpChunk2;
        private IPAddress m_ip;
        private int m_port;
        private string m_uniqueBnetName;

        private ParseCallback __LogonResponse2;
        private void HandleLogonResponse2(PD data)
        {
            DataReader dr = new DataReader(data.Data);
            int success = dr.ReadInt32();
            if (success == 0)
            {
                m_events.OnLoginSucceeded(BaseEventArgs.GetEmpty(data));

                BncsPacket pck = new BncsPacket((byte)BncsPacketId.QueryRealms2);
            }
            else
            {
                m_events.OnLoginFailed(BaseEventArgs.GetEmpty(data));
            }
        }

        private ParseCallback __QueryRealms2;
        private void HandleQueryRealms2(PD data)
        {
            DataReader dr = new DataReader(data.Data);
            dr.Seek(4);
            int count = dr.ReadInt32();
            RealmServer[] realms = new RealmServer[count];
            for (int i = 0; i < count; i++)
            {
                dr.Seek(4);
                string title = dr.ReadCString();
                string desc = dr.ReadCString();

                realms[i] = new RealmServer(title, desc);
            }

            AvailableRealmsEventArgs args = new AvailableRealmsEventArgs(realms) { EventData = data };
            OnRealmsRetrieved(args);
        }

        private ParseCallback __AuthInfo;
        private void HandleAuthInfo(PD data)
        {
            DataReader dr = new DataReader(data.Data);
            dr.Seek(4);
            m_serverToken = dr.ReadInt32();

            m_oldAuthInfo(data);
        }

        private ParseCallback __LogonRealmEx;
        private void HandleLogonRealmEx(PD data)
        {
            DataReader dr = new DataReader(data.Data);
            m_mcpCookie = dr.ReadInt32();
            int status = dr.ReadInt32();
            m_mcpStatus = status;

            if (data.Length > 8)
            {
                m_mcpChunk1 = dr.ReadByteArray(8);
                byte[] ipAddress = dr.ReadByteArray(4);
                m_ip = new IPAddress(ipAddress);
                m_port = dr.ReadInt32();
                m_mcpChunk2 = dr.ReadByteArray(48);
                m_uniqueBnetName = dr.ReadCString();

                BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
            }
            else
            {
                RealmFailedEventArgs args = new RealmFailedEventArgs((RealmFailureReason)status) { EventData = data };
                OnRealmConnectionFailed(args);
            }
        }

        partial void FreeArgumentResources(BaseEventArgs e)
        {
            if (e == null)
                return;

            BattleNetClient.ParseData data = e.EventData as BattleNetClient.ParseData;
            if (data != null && data.Data != null)
                BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }
    }
}
