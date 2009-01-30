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
        <p>Select a Channel: <span id="availableChannelsList"></span> <a href="javascript:requestAvailableChannels();">[refresh channel list]</a></p>
    </div>
    <div id="userLists" style="height: 100%;"></div>

    <div id="chatArea" style="height: 100%;">
        <div id="enterText">
        
        </div>
        <div id="scrollTo">&nbsp;</div>
    </div>
<asp:Literal id="initializationScript" runat="server" Mode="PassThrough">
<script type="text/javascript">
<!--
// just a sample; never actually gets passed through
    var channelID = "539be340-302f-442c-ae1c-f9db9b40fc75";
// -->
</script>
</asp:Literal>
        <script type="text/javascript">
    <!--

var mostRecentEvent = 0;

function OnApplicationReady()
{
    var layout = new YAHOO.widget.Layout({
        units : [
            { position: 'top', height: 40, body: 'channelSelect', collapse: true },
            { position: 'right', width: 300, body: 'userLists', animate: true, resize: false, collapse: true, close: false, scroll: true },
            { position: 'center', body: 'chatArea', scroll: true }
            ]
    });
    layout.render();

    chatWindow = new JinxBotWeb.ChatDisplay(document.getElementById('enterText'), document.getElementById('scrollTo'));
    userList = new JinxBotWeb.UserList(null, document.getElementById('userLists'));

    if (channelID)
    {
        startServicePolling();
    }
    
    requestAvailableChannels();
}

function requestAvailableChannels()
{
    var client = new JinxBotWebClient();
    client.GetAvailableChannels(AvailableChannelsRequestSucceeded, AvailableChannelsRequestFailed);
}

function AvailableChannelsRequestSucceeded(result)
{
    var avChannelsList = document.getElementById('availableChannelsList');
    while (avChannelsList.childNodes.length > 0)
        avChannelsList.removeChild(avChannelsList.childNodes[0]);
        
    for (var i = 0; i < result.length; i++)
    {
        CreateChannelLink(result[i], avChannelsList);
    }
}

function loadChannel(clientID, channelName)
{
    window.location = cleanupLocation() + "/" + channelName;
    channelID = clientID;
    startServicePolling();
}

function AvailableChannelsRequestFailed(error)
{
    alert('Could not request available channels.\n' + error);
}

function CreateChannelLink(clientChannel, container)
{
    var link = document.createElement('a');
    link.appendChild(document.createTextNode(clientChannel.ClientName + ' - ' + clientChannel.Gateway));
    link.href = cleanupLocation() + '/' + setupChannelName(clientChannel.ClientName);
    link.setAttribute('onclick', "loadChannel('" + clientChannel.ChannelID + "', '" + setupChannelName(clientChannel.ClientName) + "'); return false;");
    container.appendChild(link);
    container.appendChild(document.createTextNode(' '));
}

function cleanupLocation()
{
    var loc = window.location.toString();
    if (loc.indexOf('aspx/') > -1)
        loc = loc.substring(0, loc.indexOf('aspx/') + 4);
    return loc;
}

function setupChannelName(name)
{
    return name.replace(' ', '_');
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
