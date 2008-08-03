/// <reference name="MicrosoftAjax.js"/>
function $ArgsTypes() { }
$ArgsTypes.prototype = 
{
    User : "UserEventArgs:#BNSharp",
    ServerChat : "ServerChatEventArgs:#BNSharp",
    Information : "InformationEventArgs:#BNSharp",
    EnteredChat : "EnteredChatEventArgs:#BNSharp",
    Chat : "ChatMessageEventArgs:#BNSharp", 
    ChannelList : "ChannelListEventArgs:#BNSharp"
};
var ArgsTypes = new $ArgsTypes();

function startServicePolling()
{
    setTimeout('callService();', 500);
}

function callService()
{
    var client = new JinxBotWebClient();
    client.GetEvents(channelID, mostRecentEvent, serviceSuccess, serviceFailure);
}

function serviceSuccess(result)
{
    for (var i = 0; i < result.length; i++)
    {
        addChat(result[i]);
        chatEvents.push(result[i]);
        mostRecentEvent = result[i].EventID;
    }
    
    startServicePolling();
}

function serviceFailure(error)
{
    chatWindow.addChat(
        generateTimestamp( { Time : new Date() } ), 
        new JinxBotWeb.ChatNode(error.get_message(), 'error')
        );
        
    startServicePolling();
}
