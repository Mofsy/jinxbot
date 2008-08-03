using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using BNSharp;
using BNSharp.BattleNet.Clans;
using System.Collections.Generic;
using System.Threading;

[ServiceContract(Namespace = "")]
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class JinxBotWebClient
{
    [OperationContract]
    [WebGet(ResponseFormat = WebMessageFormat.Json)]
    public ClientChatEvent[] GetEvents(Guid channel, int lastEventID)
    {
        List<ChatEvent> chatEvents = Services.ExistingEvents[channel];
        int sleeps = 0;
        do
        {
            var eventsToSend = from ce in chatEvents
                               where ce.EventID > lastEventID && ce.ChannelID == channel
                               select ce;

            if (eventsToSend.Count() == 0)
            {
                Thread.Sleep(1000);
                sleeps++;
            }
            else
            {
                return eventsToSend.ToClient();
            }
        } while (sleeps < 30);
        return new ClientChatEvent[0];
    }
}

public static class ClientConverters
{
    public static ClientChatEvent[] ToClient(this IEnumerable<ChatEvent> chatEvents)
    {
        return chatEvents.ToList().ConvertAll(c => new ClientChatEvent(c)).ToArray();
    }
}