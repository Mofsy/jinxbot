using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using JinxBot.Plugins.UI;
using JinxBot.Reliability;
using JinxBot.Windows7;

namespace JinxBot.Configuration
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class InstanceManagementService : IJinxBotSingleInstanceService
    {
        public static bool HostIMS(IMainWindow mainWindow)
        {
            try
            {
                InstanceManagementService svc = new InstanceManagementService(mainWindow);
                Uri uri = new Uri("net.pipe://localhost/JinxBot-" + Process.GetCurrentProcess().Id.ToString());
                ServiceHost host = new ServiceHost(svc, uri);
                host.AddServiceEndpoint(typeof(IJinxBotSingleInstanceService), new NetNamedPipeBinding(), uri);
                host.Open(TimeSpan.MaxValue);
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                GlobalErrorHandler.ReportException(ex);
#endif
                return false;
            }
        }

        private IJumpListWindowTarget m_target;

        private InstanceManagementService(IMainWindow mainWindow)
        {
            m_target = mainWindow as IJumpListWindowTarget;
        }

        #region IJinxBotSingleInstanceService Members

        public void InvokeParameter(string[] param)
        {
            if (m_target != null)
                m_target.HandleJumpListCall(param);
        }

        #endregion
    }
}
