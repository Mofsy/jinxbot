using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp;
using BNSharp.Plugins;
using PD = BNSharp.Net.BattleNetClient.ParseData;
using BNSharp.MBNCSUtil;
using System.Net;

namespace JinxBot.Plugins.McpHandler
{
    public partial class McpConnection
    {
        private IBattleNetEvents m_events;
        private BattleNetClient m_client;
        private ParseCallback m_oldLogonResponse2, m_oldQueryRealms, m_oldAuthInfo, m_oldLogonRealmEx;
        private bool m_registered;
        private object m_registrationSync = new object();
        private int m_serverToken;

        public McpConnection()
        {
            __LogonResponse2 = new ParseCallback(HandleLogonResponse2);
            __QueryRealms2 = new ParseCallback(HandleQueryRealms2);
            __AuthInfo = new ParseCallback(HandleAuthInfo);
            __LogonRealmEx = new ParseCallback(HandleLogonRealmEx);
        }

        public virtual void RegisterPackets(BattleNetClient client)
        {
            lock (m_registrationSync)
            {
                if (!m_registered)
                {
                    m_client = client;

                    m_events = client.RegisterCustomPacketHandler(BncsPacketId.LogonResponse2, __LogonResponse2, out m_oldLogonResponse2);
                    client.RegisterCustomPacketHandler(BncsPacketId.QueryRealms2, __QueryRealms2, out m_oldQueryRealms);
                    client.RegisterCustomPacketHandler(BncsPacketId.AuthInfo, __AuthInfo, out m_oldAuthInfo);
                    client.RegisterCustomPacketHandler(BncsPacketId.LogonRealmEx, __LogonRealmEx, out m_oldLogonRealmEx);
                    m_registered = true;
                }
            }
        }

        public virtual void UnregisterPackets(BattleNetClient client)
        {
            lock (m_registrationSync)
            {
                if (m_registered)
                {
                    client.UnregisterCustomPacketHandler(BncsPacketId.LogonRealmEx, m_oldLogonRealmEx);
                    client.UnregisterCustomPacketHandler(BncsPacketId.AuthInfo, m_oldAuthInfo);
                    client.UnregisterCustomPacketHandler(BncsPacketId.QueryRealms2, m_oldQueryRealms);
                    client.UnregisterCustomPacketHandler(BncsPacketId.LogonResponse2, m_oldLogonResponse2);
                    m_registered = false;
                    m_client = null;
                }
            }
        }

        public virtual void LogonRealm(RealmServer server)
        {
            if (object.ReferenceEquals(server, null))
                throw new ArgumentNullException("server");

            Random r = new Random();
            int clientToken = r.Next();
            byte[] passwordHash = OldAuth.DoubleHashPassword("password", clientToken, m_serverToken);
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.LogonRealmEx);
            pck.InsertInt32(clientToken);
            pck.InsertByteArray(passwordHash);
            pck.InsertCString(server.Title);

            m_client.Send(pck);
        }
    }
}
