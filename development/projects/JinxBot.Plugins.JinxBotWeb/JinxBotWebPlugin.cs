using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using BNSharp;
using JinxBot.Plugins.JinxBotWeb.JinxBotWeb;
using System.Windows.Forms;
using System.Security.Cryptography;
using BNSharp.BattleNet;
using System.Threading;
using System.Diagnostics;

namespace JinxBot.Plugins.JinxBotWeb
{
    [JinxBotPlugin(Name = "JinxBot[Web]", Version = "0.08.07.25", Author = "MyndFyre", Description = "Enables the broadcast of your current channel to the web.")]
    public class JinxBotWebPlugin : IEventListener, IDisposable
    {
        private BattleNetClient m_client;
        private JinxBotWebApplicationClient m_svc;
        private Guid m_channelID;
        private byte[] m_pwHash = Hash("test");

        public JinxBotWebPlugin()
        {
            SetupEvents();

            m_svc = new JinxBotWebApplicationClient();
            m_channelID = new Guid("539be340-302f-442c-ae1c-f9db9b40fc75");

            SetupPosting();
        }

        private static byte[] Hash(string p)
        {
            SHA1 sha = new SHA1Managed();
            return sha.ComputeHash(Encoding.Unicode.GetBytes(p));
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
        }

        #region IEventListener Members

        public void HandleClientStartup(BattleNetClient client)
        {
            m_client = client;

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

            if (!m_svc.LoginChannel(m_channelID, m_pwHash, client.Settings.Server))
            {
                MessageBox.Show("Unable to login channel.");
            }
        }

        public void HandleClientShutdown(BattleNetClient client)
        {
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
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.UserShown });
        }

        private ChatMessageEventHandler __WhisperSent;
        void WhisperSent(object sender, ChatMessageEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.WhisperSent });
        }

        private ChatMessageEventHandler __WhisperReceived;
        void WhisperReceived(object sender, ChatMessageEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.WhisperReceived, EventData = e });
        }

        private EventHandler __WardenUnhandled;
        void WardentUnhandled(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventData = new InformationEventArgs("Warden was unhandled; the client may be disconnected."), EventType = ClientEventType.WardenUnhandled });
        }

        private ChatMessageEventHandler __UserSpoke;
        void UserSpoke(object sender, ChatMessageEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.UserSpoke });
        }

        private UserEventHandler __UserLeft;
        void UserLeft(object sender, UserEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.UserLeft, EventData = e });
        }

        private UserEventHandler __UserJoined;
        void UserJoined(object sender, UserEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.UserJoined });
        }

        private UserEventHandler __UserFlagsChanged;
        void UserFlagsChanged(object sender, UserEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.UserFlags });
        }

        private ChatMessageEventHandler __UserEmoted;
        void UserEmoted(object sender, ChatMessageEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.UserEmoted, EventData = e });
        }

        private ServerChatEventHandler __ServerErrorReceived;
        void m_client_ServerErrorReceived(object sender, ServerChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.ServerError });
        }

        private ServerChatEventHandler __ServerBroadcast;
        void ServerBroadcast(object sender, ServerChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.ServerBroadcast, EventData = e });
        }

        private ChatMessageEventHandler __MessageSent;
        void MessageSent(object sender, ChatMessageEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.MessageSent });
        }

        private EventHandler __LoginSucceeded;
        void LoginSucceeded(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.LoginSucceeded, EventData = new InformationEventArgs("Login succeeded!") });
        }

        private EventHandler __LoginFailed;
        void LoginFailed(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventData = new InformationEventArgs("Login failed!"), EventType = ClientEventType.LoginFailed });
        }

        private ServerChatEventHandler __JoinedChannel;
        void JoinedChannel(object sender, ServerChatEventArgs e)
        {
            m_svc.SetChannelName(m_channelID, Hash("test"), e.Text);
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.JoinedChannel });
        }

        private ServerChatEventHandler __InformationReceived;
        void InformationReceived(object sender, ServerChatEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
                PostEvent(new ClientEvent { EventType = ClientEventType.InformationReceived, EventData = e });
        }

        private InformationEventHandler __Information;
        void Information(object sender, InformationEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.Information });
        }

        private ErrorEventHandler __Error;
        void Error(object sender, ErrorEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.Error, EventData = e });
        }

        private EnteredChatEventHandler __EnteredChat;
        void EnteredChat(object sender, EnteredChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.EnteredChat, EventData = e });
        }

        private EventHandler __Disconnected;
        void Disconnected(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventData = new InformationEventArgs("Disconnected."), EventType = ClientEventType.Disconnected });
        }

        private EventHandler __Connected;
        void Connected(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventData = new InformationEventArgs("Connected."), EventType = ClientEventType.Connected });
        }

        private InformationEventHandler __CommandSent;
        void CommandSent(object sender, InformationEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.CommandSent, EventData = e });
        }

        private EventHandler __ClientCheckPassed;
        void ClientCheckPassed(object sender, EventArgs e)
        {
            PostEvent(new ClientEvent { EventData = new InformationEventArgs("Versioning passed!"), EventType = ClientEventType.ClientCheckPassed });
        }

        private ClientCheckFailedEventHandler __ClientCheckFailed;
        void ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.ClientCheckFailed, EventData = e });
        }

        private ServerChatEventHandler __ChannelWasRestricted;
        void ChannelWasRestricted(object sender, ServerChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.ChannelRestricted });
        }

        private ServerChatEventHandler __ChannelWasFull;
        void ChannelWasFull(object sender, ServerChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.ChannelFull, EventData = e });
        }

        private ChannelListEventHandler __ChannelListReceived;
        void ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            PostEvent(new ClientEvent { EventData = e, EventType = ClientEventType.ChannelListReceived });
        }

        private ServerChatEventHandler __ChannelDidNotExist;
        void ChannelDidNotExist(object sender, ServerChatEventArgs e)
        {
            PostEvent(new ClientEvent { EventType = ClientEventType.ChannelDidNotExist, EventData = e });
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_svc != null)
                {
                    m_svc.Close();
                    m_svc = null;
                }

                if (m_submitLoop != null)
                {
                    m_submitLoop.Abort();
                    m_submitLoop = null;
                }

                if (m_submitWait != null)
                {
                    m_submitWait.Close();
                    m_submitWait = null;
                }
            }
        }

        #endregion

        #region queued uploader
        private EventWaitHandle m_submitWait = new EventWaitHandle(false, EventResetMode.AutoReset);
        private Queue<ClientEvent> m_events = new Queue<ClientEvent>();
        private object m_queueLock = new object();

        private Thread m_submitLoop;
        private void SetupPosting()
        {
            m_submitLoop = new Thread(new ThreadStart(SubmitLoop));
            m_submitLoop.IsBackground = true;
            m_submitLoop.Priority = ThreadPriority.BelowNormal;

            m_submitLoop.Start();
        }

        private void PostEvent(ClientEvent ev)
        {
            lock (m_queueLock)
            {
                m_events.Enqueue(ev);
            }
            m_submitWait.Set();
        }

        private void SubmitLoop()
        {
            try
            {
                while (true)
                {
                    m_submitWait.Reset();

                    while (m_events.Count == 0)
                        m_submitWait.WaitOne();

                    try
                    {
                        lock (m_queueLock)
                        {
                            List<ClientEvent> eventsToSend = new List<ClientEvent>();
                            while (m_events.Count > 0)
                            {
                                ClientEvent ev = m_events.Dequeue();
                                eventsToSend.Add(ev);
                            }

                            m_svc.PostEvents(m_channelID, m_pwHash, eventsToSend.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex, "Service submission error (JinxBotWeb)");
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // exit gracefully
            }
        }
        #endregion

        #region IJinxBotPlugin Members

        public void Startup(IDictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }

        public void Shutdown(IDictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
