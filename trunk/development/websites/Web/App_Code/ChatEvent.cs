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

/// <summary>
/// Summary description for ChatEvent
/// </summary>
public class ChatEvent
{
    private int m_id;
    private BaseEventArgs m_args;

    public ChatEvent() { }

    public ChatEvent(int id, BaseEventArgs args)
    {
        m_id = id;
        m_args = args;
    }

    public int ID
    {
        get { return m_id; }
        set { m_id = value; }
    }

    public BaseEventArgs Args
    {
        get { return m_args; }
        set { m_args = value; }
    }
}
