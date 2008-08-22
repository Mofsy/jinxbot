<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Channel.aspx.cs" Inherits="Channel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" type="text/css" href="./assets/layout-core.css" />   
    <link ref="stylesheet" type="text/css" href="./assets/skins/sam/layout.css" />
    <link rel="Stylesheet" type="text/css" href="./assets/skins/sam/layout-skin.css" />
    <link rel="Stylesheet" type="text/css" href="BlizzStyles.css" />
</head>
<body class=" yui-skin-sam">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="mgr" runat="server">
        <Services>
            <asp:ServiceReference InlineScript="false" Path="~/JinxBotWebClient.svc" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/BattleNetClientScript.js" />
            <asp:ScriptReference Path="~/ChatDocumentBehaviors.js" />
            <asp:ScriptReference Path="~/scripts/yahoo-min.js" />
            <asp:ScriptReference Path="~/scripts/event-min.js" />
            <asp:ScriptReference Path="~/scripts/dom-min.js" />
            <asp:ScriptReference Path="~/scripts/element-beta-min.js" />
            <asp:ScriptReference Path="~/scripts/dragdrop-min.js" />
            <asp:ScriptReference Path="~/scripts/resize-beta-min.js" />
            <asp:ScriptReference Path="~/scripts/animation-min.js" />
            <asp:ScriptReference Path="~/scripts/layout-beta-min.js" />
        </Scripts>
    </asp:ScriptManager>
    <div id="channelSelect" style="height: 100%; overflow: hidden;">
        <p>Select a Channel:</p>
    </div>
    <div id="userLists" style="height: 100%;">
    
    </div>

    <div id="chatArea" style="height: 100%;">
        <div id="enterText">
        
        </div>
        <div id="scrollTo">&nbsp;</div>
    </div>
    
        <script type="text/javascript">
    <!--
var channelID = "539be340-302f-442c-ae1c-f9db9b40fc75";
var mostRecentEvent = 0;

function OnApplicationReady()
{
    var layout = new YAHOO.widget.Layout({
        units : [
            { position: 'top', height: 40, body: 'channelSelect', collapse: true },
            { position: 'right', width: 300, body: 'userLists', animate: true, resize: false, collapse: true, close: false },
            { position: 'center', body: 'chatArea' }
            ]
    });
    layout.render();
    
    chatWindow = new JinxBotWeb.ChatDisplay(document.getElementById('enterText'), document.getElementById('scrollTo'));
    
    startServicePolling();
}

Sys.Application.add_load(OnApplicationReady);

function DispatchEvent(clientEvent)
{
    switch (clientEvent.EventType)
    {
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ChannelDidNotExist:
            OnChannelDNE(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ChannelListReceived:
            OnChannelListReceived(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ChannelFull:
            OnChannelFull(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ChannelRestricted:
            OnChannelRestricted(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ClientCheckFailed:
            OnClientCheckFailed(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ClientCheckPassed:
            OnClientCheckPassed(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.CommandSent:
            OnCommandSent(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.Connected:
            OnConnected(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.Disconnected:
            OnDisconnected(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.EnteredChat:
            OnEnteredChat(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.Error:
            OnError(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.Information:
            OnInformation(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.InformationReceived:
            OnInformationReceived(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.JoinedChannel:
            OnJoinedChannel(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.LoginFailed:
            OnLoginFailed(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.LoginSucceeded:
            OnLoginSucceeded(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.MessageSent:
            OnMessageSent(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ServerBroadcast:
            OnServerBroadcast(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.ServerError:
            OnServerError(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserEmoted:
            OnUserEmoted(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserFlags:
            OnUserFlags(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserJoined:
            OnUserJoined(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserLeft:
            OnUserLeft(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserShown:
            OnUserShown(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.UserSpoke:
            OnUserSpoke(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.WardenUnhandled:
            OnWardenUnhandled(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.WhisperReceived:
            OnWhisperReceived(clientEvent.EventData);
            break;
        case JinxBot.Plugins.JinxBotWeb.ClientEventType.WhisperSent:
            OnWhisperSent(clientEvent.EventData);
            break;
    }
}

    // -->
    </script>
    </form>
</body>
</html>
