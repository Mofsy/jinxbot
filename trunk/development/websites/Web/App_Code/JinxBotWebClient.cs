using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using BNSharp;

/// <summary>
/// Summary description for JinxBotWebClient
/// </summary>
[WebService(Namespace = "http://www.jinxbot.net/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class JinxBotWebClient : System.Web.Services.WebService
{

    public JinxBotWebClient()
    {

    }

    [WebMethod]
    public EnteredChatEventArgs GetArgs()
    {
        return new EnteredChatEventArgs("DarkTemplar~AoA#2", "STAR", "DarkTemplar~AoA");
    }

}

