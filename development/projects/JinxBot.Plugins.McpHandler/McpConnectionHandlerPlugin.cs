using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.McpHandler
{
    [JinxBotPlugin(Author = "MyndFyre", Description = "Adds support to JinxBot for connecting to a Battle.net Realm Server for Diablo 2.", Name = "Diablo 2 Realm Plugin", Version = "0.1.0.0")]
    internal class McpConnectionHandlerPlugin : ISingleClientPlugin, IPacketHandlerPlugin
    {
        private McpConnection m_conn;
        private bool m_registered;

        public McpConnectionHandlerPlugin()
        {
            m_conn = new McpConnection();
        }

        #region IPacketHandlerPlugin Members

        public void RegisterPackets(BattleNetClient client)
        {
            Product clientProduct = Product.GetByProductCode(client.Settings.Client);
            if (object.ReferenceEquals(clientProduct, Product.Diablo2Retail) || object.ReferenceEquals(clientProduct, Product.Diablo2Expansion))
            {
                m_registered = true;
                m_conn.RegisterPackets(client);
            }
        }

        public void UnregisterPackets(BattleNetClient client)
        {
            if (m_registered)
            {
                m_conn.UnregisterPackets(client);
                m_registered = false;
            }
        }

        #endregion

        #region IJinxBotPlugin Members

        public void Startup(IDictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }

        public void Shutdown(IDictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
