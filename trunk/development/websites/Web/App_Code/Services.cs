using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

/// <summary>
/// Summary description for Services
/// </summary>
public static class Services
{
    public static ChannelServiceDataContext DataConnection;
    public static Dictionary<Guid, List<ChatEvent>> ExistingEvents = InitializeExistingEventDictionary();

    public static Dictionary<Guid, List<ChatEvent>> InitializeExistingEventDictionary()
    {
        DataConnection = new ChannelServiceDataContext();
        var channelList = from ch in DataConnection.Channels
                          select ch;

        Dictionary<Guid, List<ChatEvent>> list = new Dictionary<Guid, List<ChatEvent>>();
        foreach (Channel c in channelList)
        {
            list.Add(c.ChannelID, new List<ChatEvent>());
            var events = from ev in DataConnection.ChatEvents
                         where ev.ChannelID == c.ChannelID && ev.Time >= (DateTime.Now - new TimeSpan(1, 0, 0, 0))
                         select ev;

            list[c.ChannelID].AddRange(events);
        }

        return list;
    }
}
