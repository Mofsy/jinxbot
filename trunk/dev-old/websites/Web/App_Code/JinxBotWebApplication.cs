using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BNSharp;
using System.Diagnostics;
using System.ServiceModel.Activation;
using System.IO;
using System.Xml;
using JinxBot.Plugins.JinxBotWeb;

// NOTE: If you change the class name "JinxBotWebApplication" here, you must also update the reference to "JinxBotWebApplication" in Web.config.
[ServiceBehavior(IncludeExceptionDetailInFaults = true, ValidateMustUnderstand = true)]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class JinxBotWebApplication : IJinxBotWebApplication
{
    public static DataContractSerializer ArgsSerializer = new DataContractSerializer(typeof(BaseEventArgs));

    #region IJinxBotWebApplication Members

    public void PostEvent(Guid channelID, byte[] mainPasswordHash, ClientEvent args)
    {
        var channel = (from ch in Services.DataConnection.Channels
                       where ch.ChannelID == channelID && ch.MainPasswordHash == mainPasswordHash
                       select ch).FirstOrDefault();

        if (channel != null)
        {
            var eventCount = Services.ExistingEvents[channelID].Count;
            if (eventCount > 50)
            {
                var eventsToDelete = (from ev in Services.DataConnection.ChatEvents
                                      where ev.ChannelID == channelID
                                      orderby ev.EventID ascending
                                      select ev).Take(10);
                Services.DataConnection.ChatEvents.DeleteAllOnSubmit(eventsToDelete);
                Services.ExistingEvents[channelID].RemoveRange(0, 10);
            }

            ChatEvent newEvent = new ChatEvent() { ChannelID = channelID, EventData = Serialize(args.EventData), Type = (int)args.EventType };
            Services.DataConnection.ChatEvents.InsertOnSubmit(newEvent);
            Services.DataConnection.SubmitChanges();

            Services.ExistingEvents[channelID].Add(newEvent);
        }
    }


    public void PostEvents(Guid channelID, byte[] mainPasswordHash, ClientEvent[] args)
    {
        var channel = (from ch in Services.DataConnection.Channels
                       where ch.ChannelID == channelID && ch.MainPasswordHash == mainPasswordHash
                       select ch).FirstOrDefault();

        if (channel != null)
        {
            var eventCount = Services.ExistingEvents[channelID].Count;
            if (eventCount > 50)
            {
                var eventsToDelete = (from ev in Services.DataConnection.ChatEvents
                                      where ev.ChannelID == channelID
                                      orderby ev.EventID ascending
                                      select ev).Take(10);
                Services.DataConnection.ChatEvents.DeleteAllOnSubmit(eventsToDelete);
                Services.ExistingEvents[channelID].RemoveRange(0, 10);
            }

            foreach (ClientEvent ev in args)
            {
                ChatEvent newEvent = new ChatEvent() { ChannelID = channelID, EventData = Serialize(ev.EventData), Type = (int)ev.EventType };
                Services.DataConnection.ChatEvents.InsertOnSubmit(newEvent);
                Services.ExistingEvents[channelID].Add(newEvent);
            }
            Services.DataConnection.SubmitChanges();
        }
    }

    private string Serialize(BaseEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        using (StringWriter sw = new StringWriter(sb))
        using (XmlTextWriter xtw = new XmlTextWriter(sw))
        {
            ArgsSerializer.WriteObject(xtw, args);
        }
        return sb.ToString();
    }

    public bool CreateChannel(string clientName, byte[] mainPasswordHash, out Guid newChannelID)
    {
        newChannelID = Guid.Empty;

        int existingChannel = (from channel in Services.DataConnection.Channels
                               where channel.ClientName.ToUpper() == clientName.ToUpper()
                               select channel).Count();
        if (existingChannel > 0)
            return false;
        else
        {
            Channel chan = new Channel()
            {
                ClientName = clientName,
                MainPasswordHash = mainPasswordHash,
                StylesheetUri = string.Empty,
                AllowPasswordedChatAccess = false,
                ChatAccessPasswordHash = new byte[20],
                Gateway = "Offline",
                CurrentChannel = "Offline"
            };

            Services.DataConnection.Channels.InsertOnSubmit(chan);
            Services.DataConnection.SubmitChanges();

            newChannelID = chan.ChannelID;
            Services.ExistingEvents.Add(newChannelID, new List<ChatEvent>());

            return true;
        }
    }

    public bool LoginChannel(Guid channelID, byte[] mainPasswordHash, string gatewayName)
    {
        var channel = (from ch in Services.DataConnection.Channels
                       where ch.ChannelID == channelID && ch.MainPasswordHash == mainPasswordHash
                       select ch).FirstOrDefault();

        if (channel != null)
        {
            channel.Gateway = gatewayName;
            Services.DataConnection.SubmitChanges();
            return true;
        }
        else return false;

    }

    public bool SetChannelName(Guid channelID, byte[] mainPasswordHash, string channelName)
    {
        var channel = (from ch in Services.DataConnection.Channels
                       where ch.ChannelID == channelID && ch.MainPasswordHash == mainPasswordHash
                       select ch).FirstOrDefault();

        if (channel != null)
        {
            //channel.CurrentChannel = channelName;
            //var itemsToDelete = from ev in Services.DataConnection.ChatEvents
            //                    where ev.ChannelID == channelID
            //                    select ev;

            //Services.DataConnection.ChatEvents.DeleteAllOnSubmit(itemsToDelete);
            //Services.DataConnection.SubmitChanges();
            //if (Services.ExistingEvents.ContainsKey(channelID))
            //{
            //    Services.ExistingEvents[channelID].Clear();
            //}
            return true;
        }
        else return false;
    }

    #endregion
}
