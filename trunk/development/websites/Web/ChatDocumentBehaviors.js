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

JinxBotWeb.UserList = function(channelNameElement, usersListElement)
{
    this._channelNameElement = channelNameElement;
    this._usersListElement = usersListElement;
    this._users = [];
}

JinxBotWeb.UserList.prototype =
{
    addUser: function(user)
    {
        var element = this._createElement(user);
        if (user.Flags == BNSharp.UserFlags.ChannelOperator)
        {
            var idx = this._findIndexOfFirstNonOpsUser();
            if (idx == -1)
            {
                this._usersListElement.appendChild(element);
            }
            else
            {
                this._usersListElement.insertBefore(this._usersListElement.childNodes[idx], element);
            }
        }
        else
        {
            this._usersListElement.appendChild(element);
        }

        this._users[user.Username] = element;
    },

    removeUser: function(userName)
    {
        if (this._users[userName])
        {
            this._usersListElement.removeChild(this._users[userName]);
            this._users[userName] = false;
        }
    },

    _findIndexOfFirstNonOpsUser: function()
    {
        return (this._usersListElement.childNodex.length == 0) ? -1 : 0;
    },

    _createElement: function(user)
    {
        var userItem = document.createElement('div');
        var className = 'userListItem userProd';
        switch (user.Stats.Product.ProductCode)
        {
            case 'CHAT':
            case 'D2DV':
            case 'D2XP':
            case 'DRTL':
            case 'DSHR':
            case 'JSTR':
            case 'SEXP':
            case 'STAR':
            case 'W2BN':
            case 'W3XP':
            case 'WAR3':
                className += user.Stats.Product.ProductCode;
                break;
            default:
                className += 'CHAT';
                break;
        }

        if (user.Flags != BNSharp.UserFlags.None)
        {
            switch (user.Flags)
            {
                case BNSharp.UserFlags.SpecialGuest:
                    className += ' userFlagsGuest';
                    break;
                case BNSharp.UserFlags.Speaker:
                    className += ' userFlagsSpeaker';
                    break;
                case BNSharp.UserFlags.Squelched:
                    className += ' userFlagsSquelched';
                    break;
                case BNSharp.UserFlags.ChannelOperator:
                    className += ' userFlagsOps';
                    break;
                case BNSharp.UserFlags.BattleNetAdministrator:
                    className += ' userFlagsBnet';
                    break;
                case BNSharp.UserFlags.BlizzardRepresentative:
                    className += ' userFlagBlizz';
                    break;
                default:
                    break;
            }
        }

        var nameNode = document.createTextNode(user.Username);
        userItem.appendChild(nameNode);

        userItem.className = className;
        
        return userItem;
    }
};

JinxBotWeb.UserList.registerClass('JinxBotWeb.UserList');

function OnChannelDNE(eventData)
{
}

function OnChannelListReceived(eventData) 
{
}

function OnChannelFull(eventData)
{

}

function OnChannelRestricted(eventData)
{

}

function OnClientCheckFailed(eventData) 
{
}

function OnClientCheckPassed(eventData)
{
}

function OnCommandSent(eventData)
{

}
function OnConnected(eventData)
{

}
function OnDisconnected(eventData)
{

}

function OnEnteredChat(eventData)
{

}
function OnError(eventData)
{

}
function OnInformation(eventData)
{

}
function OnInformationReceived(eventData)
{

}
function OnJoinedChannel(eventData)
{

}
function OnLoginFailed(eventData)
{

}
            
function OnLoginSucceeded(eventData)
{

}

function OnMessageSent(eventData)
{

}

function OnServerBroadcast(eventData) 
{

}

function OnServerError(eventData)
{

}

function OnUserEmoted(eventData)
{

}

function OnUserFlags(eventData)
{

}

function OnUserJoined(eventData)
{

}

function OnUserLeft(eventData)
{

}

function OnUserShown(eventData)
{

}

function OnUserSpoke(eventData)
{

}

function OnWardenUnhandled(eventData)
{

}

function OnWhisperReceived(eventData)
{

}

function OnWhisperSent(eventData)
{

}
 
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
    userList.removeUser(event.EventData.User.Username);
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
    userList.addUser(event.EventData.User);
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