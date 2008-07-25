using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp;

namespace JinxBot.Plugins.JinxBotWeb
{
    [JinxBotPlugin(Name = "JinxBot[Web]", Version = "0.08.07.25", Author = "MyndFyre", Description = "Enables the broadcast of your current channel to the web.")]
    public class JinxBotWebPlugin : IEventListener
    {
        private BattleNetClient m_client;

        public JinxBotWebPlugin()
        {
            SetupEvents();
        }

        private void SetupEvents()
        {
            __ChannelDidNotExist = new ServerChatEventHandler(ChannelDidNotExist);
            __ChannelListReceived = new ChannelListEventHandler(ChannelListReceived);
            __ChannelWasFull = new ServerChatEventHandler(ChannelWasFull);
            __ChannelWasRestricted = new ServerChatEventHandler(ChannelWasRestricted);
            __ClientCheckFailed = new ClientCheckFailedEventHandler(ClientCheckFailed);
            __ClientCheckPassed = new EventHandler(ClientCheckPassed);
            __CommandSent = new InformationEventHandler(CommandSent);
            __Connected = new EventHandler(Connected);
            __Disconnected = new EventHandler(Disconnected);
            __EnteredChat = new EnteredChatEventHandler(EnteredChat);
            __Error = new ErrorEventHandler(Error);
            __Information = new InformationEventHandler(Information);
            __InformationReceived = new ServerChatEventHandler(InformationReceived);
            __JoinedChannel = new ServerChatEventHandler(JoinedChannel);
            __LoginFailed = new EventHandler(LoginFailed);
            __LoginSucceeded = new EventHandler(LoginSucceeded);
            __MessageSent = new ChatMessageEventHandler(MessageSent);
            __ServerBroadcast = new ServerChatEventHandler(ServerBroadcast);
            __ServerErrorReceived = new ServerChatEventHandler(m_client_ServerErrorReceived);
            __UserEmoted = new ChatMessageEventHandler(UserEmoted);
            __UserFlagsChanged = new UserEventHandler(UserFlagsChanged);
            __UserJoined = new UserEventHandler(UserJoined);
            __UserLeft = new UserEventHandler(UserLeft);
            __UserShown = new UserEventHandler(UserShown);
            __UserSpoke = new ChatMessageEventHandler(UserSpoke);
            __WardenUnhandled = new EventHandler(WardentUnhandled);
            __WhisperReceived = new ChatMessageEventHandler(WhisperReceived);
            __WhisperSent = new ChatMessageEventHandler(WhisperSent);
            __UserProfileReceived = new UserProfileEventHandler(UserProfileReceived);
        }

        #region IEventListener Members

        public void HandleClientStartup(BattleNetClient client)
        {
            m_client = client;

            m_client.RegisterUserProfileReceivedNotification(Priority.Low, __UserProfileReceived);
            m_client.RegisterChannelDidNotExistNotification(Priority.Low, __ChannelDidNotExist);
            m_client.RegisterChannelListReceivedNotification(Priority.Low, __ChannelListReceived);
            m_client.RegisterChannelWasFullNotification(Priority.Low, __ChannelWasFull);
            m_client.RegisterChannelWasRestrictedNotification(Priority.Low, __ChannelWasRestricted);
            m_client.RegisterClientCheckFailedNotification(Priority.Low, __ClientCheckFailed);
            m_client.RegisterClientCheckPassedNotification(Priority.Low, __ClientCheckPassed);
            m_client.RegisterCommandSentNotification(Priority.Low, __CommandSent);
            m_client.RegisterConnectedNotification(Priority.Low, __Connected);
            m_client.RegisterDisconnectedNotification(Priority.Low, __Disconnected);
            m_client.RegisterEnteredChatNotification(Priority.Low, __EnteredChat);
            m_client.RegisterErrorNotification(Priority.Low, __Error);
            m_client.RegisterInformationNotification(Priority.Low, __Information);
            m_client.RegisterInformationReceivedNotification(Priority.Low, __InformationReceived);
            m_client.RegisterJoinedChannelNotification(Priority.Low, __JoinedChannel);
            m_client.RegisterLoginFailedNotification(Priority.Low, __LoginFailed);
            m_client.RegisterLoginSucceededNotification(Priority.Low, __LoginSucceeded);
            m_client.RegisterMessageSentNotification(Priority.Low, __MessageSent);
            m_client.RegisterServerBroadcastNotification(Priority.Low, __ServerBroadcast);
            m_client.RegisterServerErrorReceivedNotification(Priority.Low, __ServerErrorReceived);
            m_client.RegisterUserEmotedNotification(Priority.Low, __UserEmoted);
            m_client.RegisterUserFlagsChangedNotification(Priority.Low, __UserFlagsChanged);
            m_client.RegisterUserJoinedNotification(Priority.Low, __UserJoined);
            m_client.RegisterUserLeftNotification(Priority.Low, __UserLeft);
            m_client.RegisterUserShownNotification(Priority.Low, __UserShown);
            m_client.RegisterUserSpokeNotification(Priority.Low, __UserSpoke);
            m_client.RegisterWardentUnhandledNotification(Priority.Low, __WardenUnhandled);
            m_client.RegisterWhisperReceivedNotification(Priority.Low, __WhisperReceived);
            m_client.RegisterWhisperSentNotification(Priority.Low, __WhisperSent);
        }

        public void HandleClientShutdown(BattleNetClient client)
        {
            m_client.UnregisterUserProfileReceivedNotification(Priority.Low, __UserProfileReceived);
            m_client.UnregisterChannelDidNotExistNotification(Priority.Low, __ChannelDidNotExist);
            m_client.UnregisterChannelListReceivedNotification(Priority.Low, __ChannelListReceived);
            m_client.UnregisterChannelWasFullNotification(Priority.Low, __ChannelWasFull);
            m_client.UnregisterChannelWasRestrictedNotification(Priority.Low, __ChannelWasRestricted);
            m_client.UnregisterClientCheckFailedNotification(Priority.Low, __ClientCheckFailed);
            m_client.UnregisterClientCheckPassedNotification(Priority.Low, __ClientCheckPassed);
            m_client.UnregisterCommandSentNotification(Priority.Low, __CommandSent);
            m_client.UnregisterConnectedNotification(Priority.Low, __Connected);
            m_client.UnregisterDisconnectedNotification(Priority.Low, __Disconnected);
            m_client.UnregisterEnteredChatNotification(Priority.Low, __EnteredChat);
            m_client.UnregisterErrorNotification(Priority.Low, __Error);
            m_client.UnregisterInformationNotification(Priority.Low, __Information);
            m_client.UnregisterInformationReceivedNotification(Priority.Low, __InformationReceived);
            m_client.UnregisterJoinedChannelNotification(Priority.Low, __JoinedChannel);
            m_client.UnregisterLoginFailedNotification(Priority.Low, __LoginFailed);
            m_client.UnregisterLoginSucceededNotification(Priority.Low, __LoginSucceeded);
            m_client.RegisterMessageSentNotification(Priority.Low, __MessageSent);
            m_client.RegisterServerBroadcastNotification(Priority.Low, __ServerBroadcast);
            m_client.RegisterServerErrorReceivedNotification(Priority.Low, __ServerErrorReceived);
            m_client.RegisterUserEmotedNotification(Priority.Low, __UserEmoted);
            m_client.RegisterUserFlagsChangedNotification(Priority.Low, __UserFlagsChanged);
            m_client.RegisterUserJoinedNotification(Priority.Low, __UserJoined);
            m_client.RegisterUserLeftNotification(Priority.Low, __UserLeft);
            m_client.RegisterUserShownNotification(Priority.Low, __UserShown);
            m_client.RegisterUserSpokeNotification(Priority.Low, __UserSpoke);
            m_client.RegisterWardentUnhandledNotification(Priority.Low, __WardenUnhandled);
            m_client.RegisterWhisperReceivedNotification(Priority.Low, __WhisperReceived);
            m_client.RegisterWhisperSentNotification(Priority.Low, __WhisperSent);
        }

        #endregion

        #region client-driven events
        private UserEventHandler __UserShown;
        void UserShown(object sender, UserEventArgs e)
        {
            AnnounceUser(e);
        }

        private ChatMessageEventHandler __WhisperSent;
        void WhisperSent(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("You whisper to ", Color.Magenta), new ChatNode(e.Username, Color.Magenta), new ChatNode(": ", Color.Magenta), new ChatNode(e.Text, Color.Magenta));
        }

        private ChatMessageEventHandler __WhisperReceived;
        void WhisperReceived(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Username, Color.Magenta), new ChatNode(" whispers: ", Color.Magenta), new ChatNode(e.Text, Color.Magenta));
        }

        private EventHandler __WardenUnhandled;
        void WardentUnhandled(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("WARNING: Warden was requested but unhandled.  You may be disconnected.", Color.IndianRed));
        }

        private ChatMessageEventHandler __UserSpoke;
        void UserSpoke(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("[", Color.LightBlue), new ChatNode(e.Username, Color.White), new ChatNode("]: ", Color.LightBlue), new ChatNode(e.Text, Color.White));
        }

        private UserEventHandler __UserLeft;
        void UserLeft(object sender, UserEventArgs e)
        {
            chat.AddChat(new ChatNode(e.User.Username, Color.Lime), new ChatNode(" left the channel.", Color.Yellow));
        }

        private UserEventHandler __UserJoined;
        void UserJoined(object sender, UserEventArgs e)
        {
            AnnounceUser(e);
        }

        private UserEventHandler __UserFlagsChanged;
        void UserFlagsChanged(object sender, UserEventArgs e)
        {
            if (m_client.ChannelName.Equals("The Void", StringComparison.Ordinal)) /* void view */
            {
                AnnounceUser(e);
            }
        }

        private ChatMessageEventHandler __UserEmoted;
        void UserEmoted(object sender, ChatMessageEventArgs e)
        {
            chat.AddChat(new ChatNode("<", Color.Yellow), new ChatNode(e.Username, Color.White), new ChatNode(string.Concat(" ", e.Text, ">"), Color.Yellow));
        }

        private ServerChatEventHandler __ServerErrorReceived;
        void m_client_ServerErrorReceived(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("[Error]: ", Color.Gray), new ChatNode(e.Text, Color.Orange));
        }

        private ServerChatEventHandler __ServerBroadcast;
        void ServerBroadcast(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("[Server]: ", Color.Gray), new ChatNode(e.Text, Color.Gray));
        }

        private ChatMessageEventHandler __MessageSent;
        void MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (m_inChat)
            {
                chat.AddChat(new ChatNode("[", Color.LightBlue), new ChatNode(m_userName, Color.White), new ChatNode("]: ", Color.LightBlue), new ChatNode(e.Text, Color.White));
            }
        }

        private EventHandler __LoginSucceeded;
        void LoginSucceeded(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Login succeeded!", Color.LimeGreen));
        }

        private EventHandler __LoginFailed;
        void LoginFailed(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Login failed.", Color.Red));
        }

        private ServerChatEventHandler __JoinedChannel;
        void JoinedChannel(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode("Joined Channel: ", Color.Yellow), new ChatNode(e.Text, Color.White));
            ChannelFlags flags = (ChannelFlags)e.Flags;
            if ((flags & ChannelFlags.SilentChannel) == ChannelFlags.SilentChannel)
                chat.AddChat(new ChatNode("This is a silent channel.", Color.LightBlue));

            ThreadStart ts = delegate
            {
                this.Text = string.Format(CultureInfo.CurrentCulture, "Chat Channel: {0}", e.Text);
                this.TabText = this.Text;
            };

            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }

        private ServerChatEventHandler __InformationReceived;
        void InformationReceived(object sender, ServerChatEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
                chat.AddChat(new ChatNode("[Server]: ", Color.Gray), new ChatNode(e.Text, Color.Gray));
        }

        private InformationEventHandler __Information;
        void Information(object sender, InformationEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Information, Color.LightSteelBlue));
        }

        private ErrorEventHandler __Error;
        void Error(object sender, ErrorEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Error, Color.Orange));
            if (e.IsDisconnecting)
            {
                chat.AddChat(new ChatNode("This error is causing you to disconnect.", Color.OrangeRed));
            }
        }

        private EnteredChatEventHandler __EnteredChat;
        void EnteredChat(object sender, EnteredChatEventArgs e)
        {
            Product clientProduct = Product.GetByProductCode(m_client.Settings.Client.ToUpperInvariant());
            string imgID = m_prp.Icons.GetImageIdFor(UserFlags.None, UserStats.CreateDefault(clientProduct));
            Image userImg = ProfileResourceProvider.GetForClient(m_client).Icons.GetImageFor(UserFlags.None, UserStats.CreateDefault(clientProduct));
            chat.AddChat(new ChatNode("Entered chat as ", Color.Yellow),
                new ImageChatNode(string.Concat(imgID, ".jpg"),
                    userImg, clientProduct.Name),
                new ChatNode(e.UniqueUsername, Color.White));
            m_userName = e.UniqueUsername;
            m_inChat = true;
            m_enteredChat = DateTime.Now;
        }

        private EventHandler __Disconnected;
        void Disconnected(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Disconnected.", Color.Yellow));
            m_inChat = false;
            m_userName = null;
        }

        private EventHandler __Connected;
        void Connected(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Connected!", Color.Yellow));
        }

        private InformationEventHandler __CommandSent;
        void CommandSent(object sender, InformationEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Information, Color.Teal));
        }

        private EventHandler __ClientCheckPassed;
        void ClientCheckPassed(object sender, EventArgs e)
        {
            chat.AddChat(new ChatNode("Versioning passed!", Color.LimeGreen));
        }

        private ClientCheckFailedEventHandler __ClientCheckFailed;
        void ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            chat.AddChat(new ChatNode(string.Format(CultureInfo.CurrentCulture, "Versioning check failed; {0}", e.Reason), Color.OrangeRed));
        }

        private ServerChatEventHandler __ChannelWasRestricted;
        void ChannelWasRestricted(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }

        private ServerChatEventHandler __ChannelWasFull;
        void ChannelWasFull(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }

        private ChannelListEventHandler __ChannelListReceived;
        void ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            List<ChatNode> nodesToAdd = new List<ChatNode>();
            nodesToAdd.Add(new ChatNode("Available Channels:", Color.LightSteelBlue));
            foreach (string s in e.Channels)
            {
                nodesToAdd.Add(ChatNode.NewLine);
                nodesToAdd.Add(new ChatNode(" - ", Color.Yellow));
                nodesToAdd.Add(new ChatNode(s, Color.White));
            }
            chat.AddChat(nodesToAdd);
        }

        private ServerChatEventHandler __ChannelDidNotExist;
        void ChannelDidNotExist(object sender, ServerChatEventArgs e)
        {
            chat.AddChat(new ChatNode(e.Text, Color.Red));
        }
        #endregion
    }
}
