using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BNSharp;
using System.Net.Security;
using JinxBot.Plugins.JinxBotWeb;

// NOTE: If you change the interface name "IJinxBotWebApplication" here, you must also update the reference to "IJinxBotWebApplication" in Web.config.
[ServiceContract(Namespace = "JinxBotWebServer")]
public interface IJinxBotWebApplication
{
    [OperationContract(AsyncPattern = false)]
    void PostEvent(Guid channelID, byte[] mainPasswordHash, ClientEvent args);

    [OperationContract(AsyncPattern = false)]
    void PostEvents(Guid channelID, byte[] mainPasswordHash, ClientEvent[] args);

    [OperationContract(AsyncPattern = false)]
    bool CreateChannel(string clientName, byte[] mainPasswordHash, out Guid newChannelID);

    [OperationContract(AsyncPattern = false)]
    bool LoginChannel(Guid channelID, byte[] mainPasswordHash, string gatewayName);

    [OperationContract(AsyncPattern = false)]
    bool SetChannelName(Guid channelID, byte[] mainPasswordHash, string channelName);
}
