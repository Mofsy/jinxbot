using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;

namespace JinxBot.Configuration
{
    internal sealed class InstanceManagementClient : IJinxBotSingleInstanceService
    {
        private SingleInstanceServiceProxy m_proxy;
        public InstanceManagementClient(int mainProcessId)
        {
            SingleInstanceServiceProxy proxy = new SingleInstanceServiceProxy(mainProcessId);
            m_proxy = proxy;
        }

        #region IJinxBotSingleInstanceService Members

        public void InvokeParameter(string[] param)
        {
            m_proxy.InvokeParameter(param);
        }

        #endregion

        private interface IJinxBotSingleInstanceChannel : IJinxBotSingleInstanceService, IClientChannel { }

        private class SingleInstanceServiceProxy : ClientBase<IJinxBotSingleInstanceChannel>, IJinxBotSingleInstanceService
        {
            public SingleInstanceServiceProxy(int mainProcessId) : base(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/JinxBot-" + mainProcessId))
            {

            }
            #region IJinxBotSingleInstanceService Members

            public void InvokeParameter(string[] param)
            {
                base.Channel.InvokeParameter(param);
            }

            #endregion
        }
    }
}
