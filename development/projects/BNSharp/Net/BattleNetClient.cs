using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.MBNCSUtil;
using System.Globalization;
using System.Net;
using BNSharp.BattleNet;

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
        #endregion

        #region .ctor
        /// <summary>
        /// Creates a new <see>BattleNetClient</see> with the specified settings.
        /// </summary>
        /// <param name="settings">An object containing the settings for a Battle.net connection.</param>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="settings"/> is <see langword="null" />.</exception>
        public BattleNetClient(IBattleNetSettings settings)
            : base(settings.Server, settings.Port)
        {
            m_settings = settings;

            InitializeListenState();

            InitializeParseDictionaries();
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
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="IBattleNetSettings">settings</see> associated with this connection.
        /// </summary>
        public IBattleNetSettings Settings
        {
            get { return m_settings; }
        }
        #endregion
    }
}
