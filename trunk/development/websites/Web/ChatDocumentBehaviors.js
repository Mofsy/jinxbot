/// <reference name="MicrosoftAjax.js"/>
/// <reference name="BattleNetClientScript.js" />
/// <reference name="JinxBotWebClient.svc" />
Type.registerNamespace("JinxBotWeb");

JinxBotWeb.ChatNode = function(text, cssClass)
{
    this._text = text;
    this._cssClass = cssClass;  
};

JinxBotWeb.ChatNode.prototype = {
    
    getText : function()
    {
        return this._text;
    },
    
    getCssClass : function()
    {
        return this._cssClass;
    },
    
    render : function()
    {
        if (this._text == '' && this._cssClass == '')
        {
            var element = document.createElement('br');
            return element;
        } 
        else
        {
            var element = document.createElement("span");
            element.className = this._cssClass;
            var textNode = document.createTextNode(this._text);
            element.appendChild(textNode);
            
            return element;
        }
    }
};

JinxBotWeb.ChatNode.registerClass('JinxBotWeb.ChatNode');

JinxBotWeb.ChatNode.lineBreak = function()
{
    return new JinxBotWeb.ChatNode('', '');
}

JinxBotWeb.ChatDisplay = function(enterTextElement, scrollToElement)
{
    this._contentElement = enterTextElement;
    this._scrollElement = scrollToElement;
};

JinxBotWeb.ChatDisplay.prototype = {
    
    addChat : function()
    {
        var element = document.createElement('p');
        for (var i = 0; i < arguments.length; i++)
        {
            var node = arguments[i];
            element.appendChild(node.render());
        }
        
        this._contentElement.appendChild(element);
        this.scroll();
    },
    
    addChatAll : function(nodes)
    {
        var element = document.createElement('p');
        for (var i = 0; i < nodes.length; i++)
        {
            var node = nodes[i];
            element.appendChild(node.render());
        }
        
        this._contentElement.appendChild(element);
        this.scroll();
    },
    
    scroll : function()
    {
        //if (document.selection && document.selection.type == 'None')
            this._scrollElement.scrollIntoView(false);
    }
};

JinxBotWeb.ChatDisplay.registerClass('JinxBotWeb.ChatDisplay');

function OnChannelDNE(clientEvent.EventData)
{
}

function OnChannelListReceived(clientEvent.EventData) 
{
}

function OnChannelFull(clientEvent.EventData)
{

}

function OnChannelRestricted(clientEvent.EventData)
{

}

function OnClientCheckFailed(clientEvent.EventData) 
{
}

function OnClientCheckPassed(clientEvent.EventData)
{
}

function OnCommandSent(clientEvent.EventData)
{

}
function OnConnected(clientEvent.EventData)
{

}
function OnDisconnected(clientEvent.EventData)
{

}

function OnEnteredChat(clientEvent.EventData)
{

}
function OnError(clientEvent.EventData)
{

}
function OnInformation(clientEvent.EventData)
{

}
function OnInformationReceived(clientEvent.EventData)
{

}
function OnJoinedChannel(clientEvent.EventData)
{

}
function OnLoginFailed(clientEvent.EventData)
{

}
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

function addChat(event)
{
    switch (event.EventData.__type)
    {
        case ArgsTypes.User:
            addChat_User(event);
            break;
        case ArgsTypes.ServerChat:
            addChat_ServerChatEventArgs(event);
            break;
        case ArgsTypes.Information:
            addChat_Information(event);
            break;
        case ArgsTypes.EnteredChat:
            addChat_EnteredChat(event);
            break;
        case ArgsTypes.Chat:
            addChat_ChatMessage(event);
            break;
        case ArgsTypes.ChannelList:
            addChat_ChannelListEventArgs(event);
            break;
        default:
            addChat_Debug(event);
            break;
    }
}
function addChat_EnteredChat(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode('Entering chat as ', 'information'),
        new JinxBotWeb.ChatNode(event.EventData.UniqueUsername)
        );
}

function addChat_ChatMessage(event)
{
    var timeStamp = generateTimestamp(event);
    
    switch (event.EventData.EventType)
    {
        case BNSharp.ChatEventType.WhisperReceived:
            chatWindow.addChat(
                timeStamp,
                new JinxBotWeb.ChatNode(event.EventData.Username, 'whisperReceivedName'),
                new JinxBotWeb.ChatNode(' whispers: ', 'whisper'),
                new JinxBotWeb.ChatNode(event.EventData.Text, 'whisperReceivedText')
                );
            break;
        case BNSharp.ChatEventType.WhisperSent:
            chatWindow.addChat(
                timeStamp,
                new JinxBotWeb.ChatNode('You whisper to ', 'whisper'),
                new JinxBotWeb.ChatNode(event.EventData.Username, 'whisperSentName'),
                new JinxBotWeb.ChatNode(': ', 'whisper'),
                new JinxBotWeb.ChatNode(event.EventData.Text, 'whisperSentText')
                );
            break;
        case BNSharp.ChatEventType.Talk:
            chatWindow.addChat(
                timeStamp,
                new JinxBotWeb.ChatNode(event.EventData.Username, 'talkUsername'),
                new JinxBotWeb.ChatNode(': ', 'text'),
                new JinxBotWeb.ChatNode(event.EventData.Text, 'text')
                );
            break;
        case BNSharp.ChatEventType.Emote:
            chatWindow.addChat(
                timeStamp, 
                new JinxBotWeb.ChatNode(event.EventData.Username + ' ', 'emoteUsername'),
                new JinxBotWeb.ChatNode(event.EventData.Text, 'emote')
                );
            break;
    }
}

function addChat_Debug(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode('Event type: ', 'information'),
        new JinxBotWeb.ChatNode(event.EventData.__type, 'userName'),
        new JinxBotWeb.ChatNode('; Source: ', 'information'),
        new JinxBotWeb.ChatNode(navigator.userAgent.indexOf('Firefox') != -1 ? event.EventData.toSource() : '(unparseable event)', 'timestamp')
        );
}

function addChat_Information(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode(event.EventData.Information, 'information')
        );
}

function addChat_User(event)
{
    switch (event.EventData.EventType)
    {
        case BNSharp.ChatEventType.UserInChannel:
            addChat_UserShown(event);
            break;
        case BNSharp.ChatEventType.UserJoinedChannel:
            addChat_UserJoined(event);
            break;
        case BNSharp.ChatEventType.UserLeftChannel:
            addChat_UserLeft(event)
            break;
    }
}

function addChat_UserJoined(event)
{
    announceUser(event);
}

function addChat_UserShown(event)
{
    announceUser(event);
}

function addChat_UserLeft(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode(event.EventData.User.Username, 'userName'),
        new JinxBotWeb.ChatNode(' has left the channel.', 'information')
        );
}

function announceUser(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode(event.EventData.User.Username, 'userName'),
        new JinxBotWeb.ChatNode(' has joined the channel with a ping of ', 'information'),
        new JinxBotWeb.ChatNode(event.EventData.User.Ping.toString(), 'userName'),
        new JinxBotWeb.ChatNode(', using ', 'information'),
        new JinxBotWeb.ChatNode(event.EventData.User.Stats.Product.Name, 'information')
        );
}

function generateTimestamp(event)
{
    return new JinxBotWeb.ChatNode('[' + event.Time.format('T') + '] ', 'timestamp');
}

function addChat_ServerChatEventArgs(event)
{
    switch (event.EventData.EventType)
    {
        case BNSharp.ChatEventType.Broadcast:
        case BNSharp.ChatEventType.Information:
            addChat_Broadcast(event);
            break;
        case BNSharp.ChatEventType.ChannelFull:
        case BNSharp.ChatEventType.ChannelDNE:
        case BNSharp.ChatEventType.ChannelRestricted:
        case BNSharp.ChatEventType.Error:
            addChat_ServerError(event);
            break;
        case BNSharp.ChatEventType.NewChannelJoined:
            addChat_JoinedChannel(event);
            break;
        default:
            addChat_Debug(event);
            break;
    }
}

function addChat_JoinedChannel(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode('Joining channel: ', 'information'),
        new JinxBotWeb.ChatNode(event.EventData.Text, 'channelName')
        );
}

function addChat_Broadcast(event)
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode('[Server]: ', 'userName'),
        new JinxBotWeb.ChatNode(event.EventData.Text, 'information')
        );
}

function addChat_ServerError(event) 
{
    chatWindow.addChat(
        generateTimestamp(event),
        new JinxBotWeb.ChatNode('[Error]: ', 'userName'),
        new JinxBotWeb.ChatNode(event.EventData.Text, 'error')
        );
}

function addChat_ChannelListEventArgs(event)
{
    var nodesList = [];
    nodesList.push(generateTimestamp(event));
    nodesList.push(new JinxBotWeb.ChatNode('Available channels:', 'channelStart'));
    for (var i = 0; i < event.EventData.Channels.length; i++)
    {
        nodesList.push(JinxBotWeb.ChatNode.lineBreak());
        nodesList.push(new JinxBotWeb.ChatNode(' - ', 'channelStart'));
        nodesList.push(new JinxBotWeb.ChatNode(event.EventData.Channels[i], 'channelName'));
    }
    chatWindow.addChatAll(nodesList);
}

function addChatDisplay(event, nodes)
{
    var nodesToAdd = [];
    if (true) // change to Settings.IncludeTimestamp
    {
        var now = event.Time;
        
    }
}