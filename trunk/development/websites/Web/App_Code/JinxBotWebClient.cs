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
        List<BaseEventArgs> args = new List<BaseEventArgs> {
            new ServerChatEventArgs(ChatEventType.NewChannelJoined, (int)ChannelFlags.PublicChannel, "Starcraft USA-1"),
            new UserEventArgs(ChatEventType.UserJoinedChannel, UserFlags.None, 110, "DarkTemplar~AoA", new byte[0]),
            new ClanCandidatesSearchEventArgs(ClanCandidatesSearchStatus.Success, new string[] { "DarkTemplar~AoA", "iPayBack!~AoA", "AoA" })
        };
        return args.ToArray();
    }
}
