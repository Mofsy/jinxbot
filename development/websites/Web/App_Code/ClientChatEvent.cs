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
using BNSharp;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using JinxBot.Plugins.JinxBotWeb;

/// <summary>
/// Summary description for ClientChatEvent
/// </summary>
[DataContract]
public class ClientChatEvent
{
    public ClientChatEvent(ChatEvent ev)
    {
        EventID = ev.EventID;
        EventData = JinxBotWebApplication.ArgsSerializer.ReadObject(new XmlTextReader(new StringReader(ev.EventData))) as BaseEventArgs;
        Time = ev.Time;
        EventType = (ClientEventType)ev.Type;
    }

    [DataMember]
    public long EventID { get; set; }
    [DataMember]
    public BaseEventArgs EventData { get; set; }

    [DataMember]
    public DateTime Time { get; set; }

    [DataMember]
    public ClientEventType EventType { get; set; }
}
