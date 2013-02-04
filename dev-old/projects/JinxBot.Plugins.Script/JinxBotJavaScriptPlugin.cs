using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Plugins.UI;
using Jurassic;
using Jurassic.Library;
using BNSharp;
using BNSharp.BattleNet;
using BNSharp.BattleNet.Friends;
using BNSharp.BattleNet.Stats;

namespace JinxBot.Plugins.Script
{
    [JinxBotPlugin(Author = "MyndFyre", Description = "Supports JavaScript scripting of JinxBot", Url = "http://www.jinxbot.net/", Name = "JinxBot JavaScript", Version = "1.0.0.0")]
    public class JinxBotJavaScriptPlugin : ISingleClientPlugin
    {
        private string _scriptSrc;

        private ScriptEngine _jsEngine;
        private JsHost _host;
        private IJinxBotClient _client;
        private ScriptEditor _editor;

        private FunctionInstance __accountCreated, __accountCreationFailed, __adChanged, __channelDidNotExist, __channelListReceived, __channelWasFull, __channelWasRestricted, __clientCheckFailed, __clientCheckPassed, __commandSent,
            __connected, __disconnected, __enteredChat, __error, __friendAdded, __friendListReceived, __friendMoved, __friendRemoved, __friendUpdated, __information, __informationReceived, __joinedChannel, __loginFailed, __loginSucceeded,
            __messageSent, __profileLookupFailed, __serverBroadcast, __serverErrorReceived, __serverNews, __userEmoted, __userFlagsChanged, __userJoined, __userLeft, __userProfileReceived, __userShown, __userSpoke, __warcraftProfileReceived,
            __whisperReceived, __whisperSent;

        public JinxBotJavaScriptPlugin()
        {

        }

        protected virtual void ResetScriptEngine(ScriptSource source, IJinxBotClient client)
        {
            _jsEngine = new ScriptEngine();
            _host = new JsHost(_jsEngine, client);
            _jsEngine.Execute(source);

            __accountCreated = GetFunc("accountCreated");
            __accountCreationFailed = GetFunc("accountCreationFailed");
            __adChanged = GetFunc("adChanged");
            __channelDidNotExist = GetFunc("channelDidNotExist");
            __channelListReceived = GetFunc("channelListReceived");
            __channelWasFull = GetFunc("channelWasFull");
            __channelWasRestricted = GetFunc("channelWasRestricted");
            __clientCheckFailed = GetFunc("clientCheckFailed");
            __clientCheckPassed = GetFunc("clientCheckPassed");
            __commandSent = GetFunc("commandSent");
            __connected = GetFunc("connected");
            __disconnected = GetFunc("disconnected");
            __enteredChat = GetFunc("enteredChat");
            __error = GetFunc("error");
            __friendAdded = GetFunc("friendAdded");
            __friendListReceived = GetFunc("friendListReceived");
            __friendMoved = GetFunc("friendMoved");
            __friendRemoved = GetFunc("friendRemoved");
            __friendUpdated = GetFunc("friendUpdated");
            __information = GetFunc("information");
            __informationReceived = GetFunc("informationReceived");
            __joinedChannel = GetFunc("joinedChannel");
            __loginFailed = GetFunc("loginFailed");
            __loginSucceeded = GetFunc("loginSucceeded");
            __messageSent = GetFunc("messageSent");
            __profileLookupFailed = GetFunc("profileLookupFailed");
            __serverBroadcast = GetFunc("serverBroadcast");
            __serverErrorReceived = GetFunc("serverErrorReceived");
            __serverNews = GetFunc("serverNews");
            __userEmoted = GetFunc("userEmoted");
            __userFlagsChanged = GetFunc("userFlagsChanged");
            __userJoined = GetFunc("userJoined");
            __userLeft = GetFunc("userLeft");
            __userProfileReceived = GetFunc("userProfileReceived");
            __userShown = GetFunc("userShown");
            __userSpoke = GetFunc("userSpoke");
            __warcraftProfileReceived = GetFunc("warcraftProfileReceived");
            __whisperReceived = GetFunc("whisperReceived");
            __whisperSent = GetFunc("whisperSent");
        }

        protected virtual FunctionInstance GetFunc(string name)
        {
            if (_jsEngine.HasGlobalValue(name))
                return _jsEngine.Evaluate(name) as FunctionInstance; // BUG: Jurassic cannot call _jsEngine.Evaluate<FunctionInstance>(name).
            else
                return null;
        }

        public void CreatePluginWindows(IProfileDocument profileDocument)
        {
            ScriptEditor editor = new ScriptEditor();
            _editor = editor;
            _editor.ScriptCode = _scriptSrc;
            profileDocument.AddDocument(editor);
            _editor.ScriptChanged += new EventHandler(_editor_ScriptChanged);
        }

        void _editor_ScriptChanged(object sender, EventArgs e)
        {
            _scriptSrc = _editor.ScriptCode;
            ResetScriptEngine(new StringScriptSource(_scriptSrc), _client);
        }

        public void DestroyPluginWindows(IProfileDocument profileDocument)
        {
            _editor.ScriptChanged -= _editor_ScriptChanged;
            _editor.Close();
        }

        public void RegisterEvents(IJinxBotClient profileClient)
        {
            _client = profileClient;
            ResetScriptEngine(new StringScriptSource(_scriptSrc), profileClient);

            profileClient.Client.AccountCreated += Client_AccountCreated;
            profileClient.Client.AccountCreationFailed += Client_AccountCreationFailed;
            profileClient.Client.AdChanged += Client_AdChanged;
            profileClient.Client.ChannelDidNotExist += Client_ChannelDidNotExist;
            profileClient.Client.ChannelListReceived += Client_ChannelListReceived;
            profileClient.Client.ChannelWasFull += Client_ChannelWasFull;
            profileClient.Client.ChannelWasRestricted += Client_ChannelWasRestricted;
            profileClient.Client.ClientCheckFailed += Client_ClientCheckFailed;
            profileClient.Client.ClientCheckPassed += Client_ClientCheckPassed;
            profileClient.Client.CommandSent += Client_CommandSent;
            profileClient.Client.Connected += Client_Connected;
            profileClient.Client.Disconnected += Client_Disconnected;
            profileClient.Client.EnteredChat += Client_EnteredChat;
            profileClient.Client.Error += Client_Error;
            profileClient.Client.FriendAdded += Client_FriendAdded;
            profileClient.Client.FriendListReceived += Client_FriendListReceived;
            profileClient.Client.FriendMoved += Client_FriendMoved;
            profileClient.Client.FriendRemoved += Client_FriendRemoved;
            profileClient.Client.FriendUpdated += Client_FriendUpdated;
            profileClient.Client.Information += Client_Information;
            profileClient.Client.InformationReceived += Client_InformationReceived;
            profileClient.Client.JoinedChannel += Client_JoinedChannel;
            profileClient.Client.LoginFailed += Client_LoginFailed;
            profileClient.Client.LoginSucceeded += Client_LoginSucceeded;
            profileClient.Client.MessageSent += Client_MessageSent;
            profileClient.Client.ProfileLookupFailed += Client_ProfileLookupFailed;
            profileClient.Client.ServerBroadcast += Client_ServerBroadcast;
            profileClient.Client.ServerErrorReceived += Client_ServerErrorReceived;
            profileClient.Client.ServerNews += Client_ServerNews;
            profileClient.Client.UserEmoted += Client_UserEmoted;
            profileClient.Client.UserFlagsChanged += Client_UserFlagsChanged;
            profileClient.Client.UserJoined += Client_UserJoined;
            profileClient.Client.UserLeft += Client_UserLeft;
            profileClient.Client.UserProfileReceived += Client_UserProfileReceived;
            profileClient.Client.UserShown += Client_UserShown;
            profileClient.Client.UserSpoke += Client_UserSpoke;
            profileClient.Client.WarcraftProfileReceived += Client_WarcraftProfileReceived;
            profileClient.Client.WhisperReceived += Client_WhisperReceived;
            profileClient.Client.WhisperSent += Client_WhisperSent;
        }

        #region BN# to Script event handler marshalers
        void Client_WhisperSent(object sender, ChatMessageEventArgs e)
        {
            if (__whisperSent != null)
                __whisperSent.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_WhisperReceived(object sender, ChatMessageEventArgs e)
        {
            if (__whisperReceived != null)
                __whisperReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_WarcraftProfileReceived(object sender, WarcraftProfileEventArgs e)
        {
            if (__warcraftProfileReceived != null)
                __warcraftProfileReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserSpoke(object sender, ChatMessageEventArgs e)
        {
            if (__userSpoke != null)
                __userSpoke.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserShown(object sender, UserEventArgs e)
        {
            if (__userShown != null)
                __userShown.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserProfileReceived(object sender, UserProfileEventArgs e)
        {
            if (__userProfileReceived != null)
                __userProfileReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserLeft(object sender, UserEventArgs e)
        {
            if (__userLeft != null)
                __userLeft.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserJoined(object sender, UserEventArgs e)
        {
            if (__userJoined != null)
                __userJoined.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserFlagsChanged(object sender, UserEventArgs e)
        {
            if (__userFlagsChanged != null)
                __userFlagsChanged.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_UserEmoted(object sender, ChatMessageEventArgs e)
        {
            if (__userEmoted != null)
                __userEmoted.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_ServerNews(object sender, ServerNewsEventArgs e)
        {
            if (__serverNews != null)
                __serverNews.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_ServerErrorReceived(object sender, ServerChatEventArgs e)
        {
            if (__serverErrorReceived != null)
                __serverErrorReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_ServerBroadcast(object sender, ServerChatEventArgs e)
        {
            if (__serverBroadcast != null)
                __serverBroadcast.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_ProfileLookupFailed(object sender, ProfileLookupFailedEventArgs e)
        {
            if (__profileLookupFailed != null)
                __profileLookupFailed.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (__messageSent != null)
                __messageSent.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_LoginSucceeded(object sender, EventArgs e)
        {
            if (__loginSucceeded != null)
                __loginSucceeded.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_LoginFailed(object sender, LoginFailedEventArgs e)
        {
            if (__loginFailed != null)
                __loginFailed.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_JoinedChannel(object sender, ServerChatEventArgs e)
        {
            if (__joinedChannel != null)
                __joinedChannel.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_InformationReceived(object sender, ServerChatEventArgs e)
        {
            if (__informationReceived != null)
                __informationReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_Information(object sender, InformationEventArgs e)
        {
            if (__information != null)
                __information.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_FriendUpdated(object sender, FriendUpdatedEventArgs e)
        {
            if (__friendUpdated != null)
                __friendUpdated.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_FriendRemoved(object sender, FriendRemovedEventArgs e)
        {
            if (__friendRemoved != null)
                __friendRemoved.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_FriendMoved(object sender, FriendMovedEventArgs e)
        {
            if (__friendMoved != null)
                __friendMoved.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_FriendListReceived(object sender, FriendListReceivedEventArgs e)
        {
            if (__friendListReceived != null)
                __friendListReceived.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_FriendAdded(object sender, FriendAddedEventArgs e)
        {
            if (__friendAdded != null)
                __friendAdded.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_Error(object sender, ErrorEventArgs e)
        {
            if (__error != null)
                __error.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_EnteredChat(object sender, EnteredChatEventArgs e)
        {
            if (__enteredChat != null)
                __enteredChat.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_Disconnected(object sender, EventArgs e)
        {
            if (__disconnected != null)
                __disconnected.Call(_host.ClientHost, ConvObj(e));
        }

        void Client_Connected(object sender, EventArgs e)
        {
            if (__connected != null)
            {
                __connected.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_CommandSent(object sender, InformationEventArgs e)
        {
            if (__commandSent != null)
            {
                __commandSent.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ClientCheckPassed(object sender, EventArgs e)
        {
            if (__clientCheckPassed != null)
            {
                __clientCheckPassed.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            if (__clientCheckFailed != null)
            {
                __clientCheckFailed.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ChannelWasRestricted(object sender, ServerChatEventArgs e)
        {
            if (__channelWasRestricted != null)
            {
                __channelWasRestricted.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ChannelWasFull(object sender, ServerChatEventArgs e)
        {
            if (__channelWasFull != null)
            {
                __channelWasFull.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            if (__channelListReceived != null)
            {
                __channelListReceived.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_ChannelDidNotExist(object sender, ServerChatEventArgs e)
        {
            if (__channelDidNotExist != null)
            {
                __channelDidNotExist.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_AdChanged(object sender, BNSharp.BattleNet.AdChangedEventArgs e)
        {
            if (__adChanged != null)
            {
                __adChanged.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_AccountCreationFailed(object sender, AccountCreationFailedEventArgs e)
        {
            if (__accountCreationFailed != null)
            {
                __accountCreationFailed.Call(_host.ClientHost, ConvObj(e));
            }
        }

        void Client_AccountCreated(object sender, AccountCreationEventArgs e)
        {
            if (__accountCreated != null)
            {
                __accountCreated.Call(_host.ClientHost, ConvObj(e));
            }
        }
        #endregion

        private JinxBotScriptObjectInstance ConvObj(object o)
        {
            return new JinxBotScriptObjectInstance(_jsEngine, o);
        }

        public void UnregisterEvents(IJinxBotClient profileClient)
        {
            profileClient.Client.AccountCreated -= Client_AccountCreated;
            profileClient.Client.AccountCreationFailed -= Client_AccountCreationFailed;
            profileClient.Client.AdChanged -= Client_AdChanged;
            profileClient.Client.ChannelDidNotExist -= Client_ChannelDidNotExist;
            profileClient.Client.ChannelListReceived -= Client_ChannelListReceived;
            profileClient.Client.ChannelWasFull -= Client_ChannelWasFull;
            profileClient.Client.ChannelWasRestricted -= Client_ChannelWasRestricted;
            profileClient.Client.ClientCheckFailed -= Client_ClientCheckFailed;
            profileClient.Client.ClientCheckPassed -= Client_ClientCheckPassed;
            profileClient.Client.CommandSent -= Client_CommandSent;
            profileClient.Client.Connected -= Client_Connected;
            profileClient.Client.Disconnected -= Client_Disconnected;
            profileClient.Client.EnteredChat -= Client_EnteredChat;
            profileClient.Client.Error -= Client_Error;
            profileClient.Client.FriendAdded -= Client_FriendAdded;
            profileClient.Client.FriendListReceived -= Client_FriendListReceived;
            profileClient.Client.FriendMoved -= Client_FriendMoved;
            profileClient.Client.FriendRemoved -= Client_FriendRemoved;
            profileClient.Client.FriendUpdated -= Client_FriendUpdated;
            profileClient.Client.Information -= Client_Information;
            profileClient.Client.InformationReceived -= Client_InformationReceived;
            profileClient.Client.JoinedChannel -= Client_JoinedChannel;
            profileClient.Client.LoginFailed -= Client_LoginFailed;
            profileClient.Client.LoginSucceeded -= Client_LoginSucceeded;
            profileClient.Client.MessageSent -= Client_MessageSent;
            profileClient.Client.ProfileLookupFailed -= Client_ProfileLookupFailed;
            profileClient.Client.ServerBroadcast -= Client_ServerBroadcast;
            profileClient.Client.ServerErrorReceived -= Client_ServerErrorReceived;
            profileClient.Client.ServerNews -= Client_ServerNews;
            profileClient.Client.UserEmoted -= Client_UserEmoted;
            profileClient.Client.UserFlagsChanged -= Client_UserFlagsChanged;
            profileClient.Client.UserJoined -= Client_UserJoined;
            profileClient.Client.UserLeft -= Client_UserLeft;
            profileClient.Client.UserProfileReceived -= Client_UserProfileReceived;
            profileClient.Client.UserShown -= Client_UserShown;
            profileClient.Client.UserSpoke -= Client_UserSpoke;
            profileClient.Client.WarcraftProfileReceived -= Client_WarcraftProfileReceived;
            profileClient.Client.WhisperReceived -= Client_WhisperReceived;
            profileClient.Client.WhisperSent -= Client_WhisperSent;
        }

        public void Startup(IDictionary<string, string> settings)
        {
            string script;
            settings.TryGetValue("script", out script);

            script = script ?? string.Empty;

            _scriptSrc = script;
        }

        public void Shutdown(IDictionary<string, string> settings)
        {
            settings["script"] = _scriptSrc;
        }

        public object GetSettingsObject()
        {
            return null;
        }

        public IPluginUpdateManifest CheckForUpdates()
        {
            return null;
        }

        public void Dispose()
        {
            
        }
    }
}
