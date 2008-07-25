using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using BNSharp;
using BNSharp.BattleNet.Clans;
using System.Collections.Generic;

[ServiceContract(Namespace = "")]
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class JinxBotWebClient
{
    [OperationContract]
    [WebGet(ResponseFormat = WebMessageFormat.Json)]
    public BaseEventArgs[] GetArgs()
    {
        List<BaseEventArgs> args = new List<BaseEventArgs>();
        return args.ToArray();
    }
}
