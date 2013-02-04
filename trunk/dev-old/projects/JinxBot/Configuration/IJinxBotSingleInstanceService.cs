using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace JinxBot.Configuration
{
    [ServiceContract]
    internal interface IJinxBotSingleInstanceService
    {
        [OperationContract(IsOneWay = true)]
        void InvokeParameter(string[] param);
    }
}
