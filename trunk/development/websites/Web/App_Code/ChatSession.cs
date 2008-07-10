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
/// Summary description for ChatSession
/// </summary>
public class ChatSession
{
    private List<ChatEvent> m_events;
    private int m_curID;
    private object m_syncObject = new object();

    public ChatSession()
    {
        m_events = new List<ChatEvent>();
    }
}
