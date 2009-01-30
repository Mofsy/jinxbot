using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.MBNCSUtil;
using BNSharp.Plugins;

namespace JinxBot.Plugins.BNSharp
{
    internal class Socks4ProxyConnector : IProxyConnector
    {
        private ClientProfile m_profile;
        private IProxiedRealConnection m_con;

        public Socks4ProxyConnector(ClientProfile settings)
        {
            m_profile = settings;
        }

        #region IProxyConnector Members

        public void Initialize(IProxiedRealConnection client)
        {
            m_con = client;
        }

        public bool Negotiate()
        {
            DataBuffer init = new DataBuffer();
            init.InsertByte(4);
            init.InsertByte(1);
            byte[] port = BitConverter.GetBytes(m_profile.Port);
            init.InsertByte(port[1]);
            init.InsertByte(port[0]);
            init.InsertByteArray(m_con.ResolveEndPoint(m_profile.Server, m_profile.Port).Address.GetAddressBytes());
            init.InsertCString("test@test.com");

            m_con.Send(init.UnderlyingBuffer, 0, init.Count);

            byte[] result = new byte[8];
            result = m_con.Receive(result, 0, 8);
            return result[1] == 0x5a;
        }

        public System.Net.IPEndPoint ResolveEndPoint(string host, int port)
        {
            return m_con.ResolveEndPoint("jinxbot.homeip.net", 9000);
        }

        public byte[] Receive(byte[] buffer, int index, int length)
        {
            return m_con.Receive(buffer, index, length);
        }

        public byte[] Receive()
        {
            return m_con.Receive();
        }

        public void Send(byte[] data, int index, int length)
        {
            m_con.Send(data, index, length);
        }

        #endregion
    }
}
