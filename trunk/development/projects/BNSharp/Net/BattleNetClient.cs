using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.MBNCSUtil;
using System.Globalization;
using System.Net;
using BNSharp.BattleNet;
using System.Collections.ObjectModel;
using System.Threading;

namespace BNSharp.Net
{
    /// <summary>
    /// Implements a client connection to Battle.net.
    /// </summary>
    /// <remarks>
    /// <para>This is the primary class that should be used when implementing a Battle.net client.  To implement one, you only need to implement
    /// the <see>IBattleNetSettings</see> interface, which provides information about a connection to Battle.net.  Once this interface is implemented
    /// and this object is created, the client should register for events and that's it.</para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public partial class BattleNetClient : ConnectionBase
    {
        #region partial methods that exist in the other partial files
        partial void InitializeListenState();
        partial void InitializeParseDictionaries();
        #endregion

        #region fields
        private IBattleNetSettings m_settings;
        private bool m_closing;
        private Dictionary<string, ChatUser> m_namesToUsers = new Dictionary<string, ChatUser>();
        private Dictionary<int, UserProfileRequest> m_profileRequests = new Dictionary<int, UserProfileRequest>();
        private int m_currentProfileRequestID;
        private string m_channelName;
        #endregion

        #region .ctor
        /// <summary>
        /// Creates a new <see>BattleNetClient</see> with the specified settings.
        /// </summary>
        /// <param name="settings">An object containing the settings for a Battle.net connection.</param>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="settings"/> is <see langword="null" />.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public BattleNetClient(IBattleNetSettings settings)
            : base(settings.Server, settings.Port)
        {
            m_settings = settings;
            m_priorityProvider = new CombinedPacketPriorityProvider();

            InitializeListenState();

            InitializeParseDictionaries();

            CreateEventThreads();
        }
        #endregion

        #region virtual methods
        /// <summary>
        /// Sends a data buffer to the server.
        /// </summary>
        /// <param name="packet">The buffer to send.</param>
        /// <remarks>
        /// <para>Use of this method is preferred because, after sending the buffer, it frees the buffer from the outgoing buffer pool.</para>
        /// </remarks>
        public virtual void Send(DataBuffer packet)
        {
            Send(packet.UnderlyingBuffer, 0, packet.Count);
            BattleNetClientResources.OutgoingBufferPool.FreeBuffer(packet.UnderlyingBuffer);
        }

        /// <summary>
        /// Begins the connection to Battle.net.
        /// </summary>
        /// <returns><see langword="true" /> if the connection succeeded; otherwise <see langword="false" />.</returns>
        public override bool Connect()
        {
            BattleNetClientResources.RegisterClient(this);

            bool ok = base.Connect();
            if (ok)
            {
                InitializeListenState();

                CultureInfo ci = CultureInfo.CurrentCulture;
                RegionInfo ri = RegionInfo.CurrentRegion;
                TimeSpan ts = DateTime.UtcNow - DateTime.Now;

                OnConnected(BaseEventArgs.GetEmpty(null));

                Send(new byte[] { 1 });

                BncsPacket pck = new BncsPacket((byte)BncsPacketId.AuthInfo);
                pck.Insert(0);
                pck.InsertDwordString("IX86"); // platform
                pck.InsertDwordString(m_settings.Client); // product
                pck.InsertInt32(m_settings.VersionByte); // verbyte
                pck.InsertDwordString(string.Concat(ci.TwoLetterISOLanguageName, ri.TwoLetterISORegionName));
                pck.InsertByteArray(LocalEP.Address.GetAddressBytes());
                pck.InsertInt32((int)ts.TotalMinutes);
                pck.InsertInt32(ci.LCID);
                pck.InsertInt32(ci.LCID);
                pck.InsertCString(ri.ThreeLetterWindowsRegionName);
                pck.InsertCString(ri.DisplayName);

                Send(pck);

                if (Settings.PingMethod == PingType.ZeroMs)
                {
                    pck = new BncsPacket((byte)BncsPacketId.Ping);
                    pck.InsertInt32(new Random().Next());
                    Send(pck);
                }

                StartParsing();

                StartListening();
            }

            return ok;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (!m_closing)
            {
                m_closing = true;

                BattleNetClientResources.UnregisterClient(this);

                OnDisconnected(BaseEventArgs.GetEmpty(null));

                ResetConnectionState();

                StopParsingAndListening();
            }
            else 
                m_closing = false;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (IsConnected)
                    Close();

                if (m_parseWait != null)
                {
                    m_parseWait.Close();
                    m_parseWait = null;
                }

                if (m_tmr != null)
                {
                    m_tmr.Dispose();
                    m_tmr = null;
                }

                CloseEventThreads();
            }
        }

        /// <summary>
        /// Sends a textual message to the server.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if the client is not connected.</exception>
        /// <exception cref="ProtocolViolationException">Thrown if <paramref name="text"/> is longer than 223 characters.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <see langword="null" />.</exception>
        public virtual void SendMessage(string text)
        {
            if (object.ReferenceEquals(null, text))
                throw new ArgumentNullException("text");

            if (text.Length > 223)
                throw new ProtocolViolationException("Maximum text length is 223 characters.");

            if (IsConnected)
            {
                BncsPacket pck = new BncsPacket((byte)BncsPacketId.ChatCommand);
                pck.InsertCString(text);
                Send(pck);
                if (text.StartsWith("/me ", StringComparison.OrdinalIgnoreCase) || text.StartsWith("/emote ", StringComparison.OrdinalIgnoreCase))
                {
                    ChatMessageEventArgs cme = new ChatMessageEventArgs(ChatEventType.Emote, UserFlags.None, "Me", text.Substring(text.IndexOf(' ') + 1));
                    OnMessageSent(cme);
                }
                else if (text.StartsWith("/", StringComparison.Ordinal))
                {
                    OnCommandSent(new InformationEventArgs(text));
                }
                else
                {
                    ChatMessageEventArgs cme = new ChatMessageEventArgs(ChatEventType.Talk, UserFlags.None, "Me", text);
                    OnMessageSent(cme);
                }
            }
            else
            {
                throw new InvalidOperationException("The client must be connected in order to send a message.");
            }
        }

        /// <summary>
        /// Creates a new account, attempting to use the login information provided in the settings.
        /// </summary>
        public virtual void CreateAccount()
        {
            bool isClientWar3 = (m_settings.Client.Equals(Product.Warcraft3Retail.ProductCode, StringComparison.Ordinal) || m_settings.Client.Equals(Product.Warcraft3Expansion.ProductCode, StringComparison.Ordinal));
            if (isClientWar3)
            {
                CreateAccountNLS();
            }
            else
            {
                CreateAccountOld();
            }
        }

        /// <summary>
        /// Allows the client to continue logging in if the login has stopped due to a non-existent username or password.
        /// </summary>
        /// <remarks>
        /// <para>If a <see>LoginFailed</see> event occurs, the client is not automatically disconnected.  The UI can then present an interface
        /// by which the user may modify the client's <see>Settings</see> instance with proper login information.  Once this has been done, the 
        /// user may then call this method to attempt to log in again.</para>
        /// <para>This method does not need to be called after the <see>AccountCreated</see> event.</para>
        /// </remarks>
        public virtual void ContinueLogin()
        {
            bool isClientWar3 = (m_settings.Client.Equals(Product.Warcraft3Retail.ProductCode, StringComparison.Ordinal) || m_settings.Client.Equals(Product.Warcraft3Expansion.ProductCode, StringComparison.Ordinal));
            if (isClientWar3)
            {
                LoginAccountNLS();
            }
            else
            {
                LoginAccountOld();
            }
        }

        #region helpers
        private void LoginAccountOld()
        {
            switch (m_settings.Client)
            {
                case "W2BN":
                    BncsPacket pck0x29 = new BncsPacket((byte)BncsPacketId.LogonResponse);
                    pck0x29.Insert(m_clientToken);
                    pck0x29.Insert(m_srvToken);
                    pck0x29.InsertByteArray(OldAuth.DoubleHashPassword(m_settings.Password, m_clientToken, m_srvToken));
                    pck0x29.InsertCString(m_settings.Username);

                    Send(pck0x29);
                    break;
                case "STAR":
                case "SEXP":
                case "D2DV":
                case "D2XP":
                    BncsPacket pck0x3a = new BncsPacket((byte)BncsPacketId.LogonResponse2);
                    pck0x3a.Insert(m_clientToken);
                    pck0x3a.Insert(m_srvToken);
                    pck0x3a.InsertByteArray(OldAuth.DoubleHashPassword(
                        m_settings.Password,
                        m_clientToken, m_srvToken));
                    pck0x3a.InsertCString(m_settings.Username);

                    Send(pck0x3a);
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Client '{0}' is not supported with old-style account login.", m_settings.Client));
            }
        }

        private void LoginAccountNLS()
        {
            m_nls = new NLS(m_settings.Username, m_settings.Password);

            BncsPacket pck0x53 = new BncsPacket((byte)BncsPacketId.AuthAccountLogon);
            m_nls.LoginAccount(pck0x53);
            Send(pck0x53);
        }


        private void CreateAccountOld()
        {
            byte[] passwordHash = OldAuth.HashPassword(m_settings.Password);
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.CreateAccount2);
            pck.InsertByteArray(passwordHash);
            pck.InsertCString(m_settings.Username);

            Send(pck);
        }

        private void CreateAccountNLS()
        {
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.AuthAccountCreate);
            m_nls = new NLS(m_settings.Username, m_settings.Password);
            m_nls.CreateAccount(pck);

            Send(pck);
        }
        #endregion
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="IBattleNetSettings">settings</see> associated with this connection.
        /// </summary>
        public IBattleNetSettings Settings
        {
            get { return m_settings; }
        }

        /// <summary>
        /// Gets a read-only list of all of the users in the current channel.
        /// </summary>
        public ReadOnlyCollection<ChatUser> Channel
        {
            get
            {
                List<ChatUser> users = new List<ChatUser>(m_namesToUsers.Values);
                return new ReadOnlyCollection<ChatUser>(users);
            }
        }

        /// <summary>
        /// Requests a user's profile.
        /// </summary>
        /// <param name="accountName">The name of the user for whom to request information.</param>
        /// <param name="profile">The profile request, which should contain the keys to request.</param>
        public void RequestUserProfile(string accountName, UserProfileRequest profile)
        {
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ReadUserData);
            pck.InsertInt32(1);
            pck.InsertInt32(profile.Count);
            int currentRequest = Interlocked.Increment(ref m_currentProfileRequestID);
            pck.InsertInt32(currentRequest);
            pck.InsertCString(accountName);
            foreach (UserProfileKey key in profile)
            {
                pck.InsertCString(key.Key);
            }

            m_profileRequests.Add(currentRequest, profile);

            Send(pck);
        }

        /// <summary>
        /// Gets the name of the current channel.
        /// </summary>
        public string ChannelName
        {
            get { return m_channelName; }
        }
        #endregion
    }
}
