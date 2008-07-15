using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using BNSharp.MBNCSUtil;
using BNSharp.MBNCSUtil.Net;
using System.IO;
using BNSharp.MBNCSUtil.Data;
using BNSharp.Plugins;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using Tmr = System.Timers.Timer;
using BNSharp.BattleNet;

namespace BNSharp.Net
{
    partial class BattleNetClient
    {
        private PriorityQueue<ParseData> m_packetQueue;
        private Thread m_listener, m_parser;
        private EventWaitHandle m_parseWait;
        private Dictionary<BncsPacketId, Priority> m_packetToPriorityMap;
        private Tmr m_tmr;
        private Dictionary<BncsPacketId, ParseCallback> m_packetToParserMap;

        partial void InitializeParseDictionaries()
        {
            m_customEventSink = new EventSink(this);

            m_packetToParserMap = new Dictionary<BncsPacketId, ParseCallback>()
            {
                { BncsPacketId.EnterChat, new ParseCallback(HandleEnterChat) },
                { BncsPacketId.GetChannelList, new ParseCallback(HandleGetChannelList) },
                { BncsPacketId.ChatEvent, new ParseCallback(HandleChatEvent) },
                { BncsPacketId.Ping, new ParseCallback(HandlePing) },
                { BncsPacketId.LogonResponse, new ParseCallback(HandleLogonResponse) },
                { BncsPacketId.LogonResponse2, new ParseCallback(HandleLogonResponse2) },
                { BncsPacketId.WarcraftGeneral, new ParseCallback(HandleWarcraftGeneral) },
                { BncsPacketId.NewsInfo, new ParseCallback(HandleNewsInfo) },
                { BncsPacketId.AuthInfo, new ParseCallback(HandleAuthInfo) },
                { BncsPacketId.AuthCheck, new ParseCallback(HandleAuthCheck) },
                { BncsPacketId.AuthAccountCreate, new ParseCallback(HandleAuthAccountCreate) },
                { BncsPacketId.AuthAccountLogon, new ParseCallback(HandleAuthAccountLogon) },
                { BncsPacketId.AuthAccountLogonProof, new ParseCallback(HandleAuthAccountLogonProof) },
                { BncsPacketId.AuthAccountChange, new ParseCallback(HandleAuthAccountChange) },
                { BncsPacketId.AuthAccountChangeProof, new ParseCallback(HandleAuthAccountChangeProof) },
                { BncsPacketId.Warden, new ParseCallback(HandleWarden) },
                { BncsPacketId.FriendsList, new ParseCallback(HandleFriendsList) },
                { BncsPacketId.FriendsAdd, new ParseCallback(HandleFriendAdded) },
                { BncsPacketId.FriendsUpdate, new ParseCallback(HandleFriendUpdate) },
                { BncsPacketId.FriendsRemove, new ParseCallback(HandleFriendRemoved) },
                { BncsPacketId.FriendsPosition, new ParseCallback(HandleFriendMoved) },
                { BncsPacketId.ClanFindCandidates, new ParseCallback(HandleClanFindCandidates) },
                { BncsPacketId.ClanInviteMultiple, new ParseCallback(HandleClanInviteMultiple) },
                { BncsPacketId.ClanCreationInvitation, new ParseCallback(HandleClanCreationInvitation) },
                { BncsPacketId.ClanDisband, new ParseCallback(HandleDisbandClan) },
                { BncsPacketId.ClanMakeChieftan, new ParseCallback(HandleClanMakeChieftan) },
                { BncsPacketId.ClanInfo, new ParseCallback(HandleClanInfo) },
                { BncsPacketId.ClanQuitNotify, new ParseCallback(HandleClanQuitNotify) },
                { BncsPacketId.ClanInvitation, new ParseCallback(HandleClanInvitation) },
                { BncsPacketId.ClanRemoveMember, new ParseCallback(HandleClanRemoveMember) },
                { BncsPacketId.ClanInvitationResponse, new ParseCallback(HandleClanInvitationResponse) },
                { BncsPacketId.ClanRankChange, new ParseCallback(HandleClanRankChange) },
                { BncsPacketId.ClanMOTD, new ParseCallback(HandleClanMotd) },
                { BncsPacketId.ClanMemberList, new ParseCallback(HandleClanMemberList) },
                { BncsPacketId.ClanMemberRemoved, new ParseCallback(HandleClanMemberRemoved) },
                { BncsPacketId.ClanMemberStatusChanged, new ParseCallback(HandleClanMemberStatusChanged) },
                { BncsPacketId.ClanMemberRankChange, new ParseCallback(HandleClanRankChange) },
                { BncsPacketId.ClanMemberInformation, new ParseCallback(HandleClanMemberInformation) }
            };
        }

        /// <summary>
        /// This is the listening thread.  All it does is loop on receive.
        /// </summary>
        private void Listen()
        {
            byte[] header = new byte[4];
            try
            {
                while (IsConnected)
                {
                    byte[] hdr = Receive(header, 0, 4);
                    if (hdr == null) return; // disconnected.
                    byte[] result = null;
                    ushort length = BitConverter.ToUInt16(hdr, 2);
                    if (length > 4)
                    {
                        if (length > BattleNetClientResources.IncomingBufferPool.BufferLength)
                            throw new ProtocolViolationException(Strings.ReceivedTooLongMessage);

                        byte[] data = BattleNetClientResources.IncomingBufferPool.GetBuffer();
                        result = Receive(data, 0, unchecked((ushort)(length - 4)));
                        if (result == null) return; // disconnected.
                    }
                    else if (length == 4)
                    {
                        length = unchecked((ushort)(length - 4));
                        result = new byte[0];
                    }
                    else
                    {
                        throw new ProtocolViolationException(Strings.InvalidPacketSizeFromBnet);
                    }
                    ParseData parseData = new ParseData(hdr[1], length, result);
                    Priority priority = DeterminePriority((BncsPacketId)parseData.PacketID);
                    m_packetQueue.Enqueue(priority, parseData);
                    m_parseWait.Set();
                }
            }
            catch (ThreadAbortException)
            {
                // exit the thread gracefully.
            }
        }

        /// <summary>
        /// This is the parsing thread.  
        /// </summary>
        private void Parse()
        {
            try
            {
                while (IsConnected)
                {
                    m_parseWait.Reset();

                    while (m_packetQueue.Count == 0)
                    {
                        m_parseWait.WaitOne();
                    }

                    ParseData data = m_packetQueue.Dequeue();
                    if (m_packetToParserMap.ContainsKey(data.PacketID))
                    {
                        m_packetToParserMap[data.PacketID](data);
                    }
                    else
                    {
                        switch (data.PacketID)
                        {
                            #region SID_NULL
                            case BncsPacketId.Null:
                                break;
                            #endregion
                            default:
                                Trace.WriteLine(data.PacketID, "Unhandled packet");
                                if (!BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data))
                                {
                                    Debug.WriteLine(data.PacketID, "Incoming buffer was not freed for packet");
                                }
                                break;
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // exit the thread gracefully.
            }
        }

        private List<NewsEntry> m_news;
        private void HandleNewsInfo(ParseData data)
        {
            DateTime UNIX_EPOCH = new DateTime(1970, 1, 1).ToUniversalTime();

            DataReader dr = new DataReader(data.Data);
            int numEntriesInPacket = dr.ReadByte();
            dr.ReadInt32(); // last login timestamp
            dr.Seek(8); // oldest news, newest news timestamps.
            //int oldestNews = dr.ReadInt32();
            //int newestNews = dr.ReadInt32();

            //DateTime oldestNewsDateUtc = UNIX_EPOCH + TimeSpan.FromSeconds(oldestNews);
            //DateTime oldestNewsDateLocal = oldestNewsDateUtc.ToLocalTime();
            //DateTime newestNewsDateUtc = UNIX_EPOCH + TimeSpan.FromSeconds(newestNews);
            //DateTime newestNewsDateLocal = newestNewsDateUtc.ToLocalTime();

            lock (m_news)
            {
                for (int i = 0; i < numEntriesInPacket; i++)
                {
                    int newsTS = dr.ReadInt32();
                    DateTime newsDateUtc = UNIX_EPOCH + TimeSpan.FromSeconds(newsTS);
                    if (newsTS == 0)
                        newsDateUtc = DateTime.UtcNow;

                    string text = dr.ReadCString();
                    NewsEntry news = new NewsEntry(newsDateUtc, text);
                    OnServerNews(new ServerNewsEventArgs(news));
                }

                BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
            }
        }

        #region connection state fields
        // these are state fields that manage connection state.  for example, m_firstChannelList
        // determines whether the client has received the first channel list received for this connection, and if so,
        // proceeds through the normal connection process.
        private bool m_firstChannelList; // determines if the first channel list has arrived yet
        private bool m_received0x50; // whether packet 0x50 has been received yet.
        private DataBuffer m_pingPck;
        private uint m_loginType, m_srvToken, m_udpVal, m_clientToken;
        private long m_mpqFiletime;
        private string m_versioningFilename, m_prodCode;
        private bool m_usingLockdown;
        private IWardenModule m_warden;
        private byte[] m_ldValStr, m_ldDigest, m_w3srv;
        private string m_valString;
        private NLS m_nls;

        private void ResetConnectionState()
        {
            m_tmr.Stop();

            m_news.Clear();

            m_firstChannelList = false;
            m_received0x50 = false;
            if (m_pingPck != null)
            {
                BattleNetClientResources.OutgoingBufferPool.FreeBuffer(m_pingPck.UnderlyingBuffer);
                m_pingPck = null;
            }
            m_loginType = m_srvToken = m_udpVal = m_clientToken = 0;
            m_mpqFiletime = 0;
            m_versioningFilename = null;
            m_prodCode = null;
            m_usingLockdown = false;

            if (m_warden != null)
            {
                m_warden.UninitWarden();
                m_warden = null;
            }
            m_ldDigest = null;
            m_ldValStr = null;
            m_w3srv = null;
            m_valString = null;

            ResetFriendsState();
            ResetClanState();
        }

        partial void ResetFriendsState();
        partial void ResetClanState();
        #endregion

        private void HandleEnterChat(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            EnteredChatEventArgs e = new EnteredChatEventArgs(dr.ReadCString(), dr.ReadCString(), dr.ReadCString());
            e.EventData = data;
            OnEnteredChat(e);

            if (m_settings.Client.Equals("WAR3", StringComparison.Ordinal) ||
                m_settings.Client.Equals("W3XP", StringComparison.Ordinal))
            {
                BncsPacket pck = new BncsPacket((byte)BncsPacketId.WarcraftGeneral);
                pck.InsertByte((byte)WarcraftCommands.IconListRequest);
                pck.InsertInt32(1);

                Send(pck);

                pck = new BncsPacket((byte)BncsPacketId.NewsInfo);
                pck.InsertInt32(0);
                Send(pck);

                RequestChannelList();
            }
        }

        private void HandleGetChannelList(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);

            List<string> channelList = new List<string>();
            string channel;
            do
            {
                channel = dr.ReadCString();
                if (channel != null && channel.Length > 0)
                    channelList.Add(channel);
            } while (channel != null && channel.Length > 0);

            ChannelListEventArgs e = new ChannelListEventArgs(channelList.ToArray());
            e.EventData = data;
            OnChannelListReceived(e);

            if (!m_firstChannelList)
            {
                m_firstChannelList = true;

                BncsPacket pckJoinChan = new BncsPacket((byte)BncsPacketId.JoinChannel);
                if (m_settings.Client.Equals("D2DV", StringComparison.Ordinal) 
                    || m_settings.Client.Equals("D2XP", StringComparison.Ordinal))
                    pckJoinChan.InsertInt32((int)ChannelJoinFlags.Diablo2FirstJoin);
                else
                    pckJoinChan.InsertInt32((int)ChannelJoinFlags.FirstJoin);

                switch (m_settings.Client)
                {
                    case "STAR":
                    case "SEXP":
                    case "W2BN":
                    case "D2DV":
                    case "D2XP":
                    case "JSTR":
                        pckJoinChan.InsertCString(m_settings.Client);
                        break;
                    case "WAR3":
                    case "W3XP":
                        pckJoinChan.InsertCString("W3");
                        break;
                }

                Send(pckJoinChan);
            }
        }

        #region chat events
        private void HandleChatEvent(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            ChatEventType type = (ChatEventType)dr.ReadInt32();
            int flags = dr.ReadInt32();
            int ping = dr.ReadInt32();
            dr.Seek(12);
            string user = dr.ReadCString();
            byte[] userInfo = dr.ReadNullTerminatedByteArray();
            string text = Encoding.ASCII.GetString(userInfo);

            switch (type)
            {
                case ChatEventType.UserFlagsChanged:
                case ChatEventType.UserInChannel:
                case ChatEventType.UserJoinedChannel:
                case ChatEventType.UserLeftChannel:
                    UserEventArgs uArgs = new UserEventArgs(type, (UserFlags)flags, ping, user, userInfo);
                    HandleUserChatEvent(uArgs);
                    break;
                case ChatEventType.Emote:
                case ChatEventType.Talk:
                case ChatEventType.WhisperReceived:
                case ChatEventType.WhisperSent:
                    ChatMessageEventArgs cmArgs = new ChatMessageEventArgs(type, (UserFlags)flags, user, text);
                    HandleChatMessageEvent(cmArgs);
                    break;
                case ChatEventType.NewChannelJoined:
                    ServerChatEventArgs joinArgs = new ServerChatEventArgs(type, flags, text);
                    OnJoinedChannel(joinArgs);
                    break;
                case ChatEventType.Broadcast:
                case ChatEventType.ChannelDNE:
                case ChatEventType.ChannelFull:
                case ChatEventType.ChannelRestricted: 
                case ChatEventType.Error:
                case ChatEventType.Information:
                    ServerChatEventArgs scArgs = new ServerChatEventArgs(type, flags, text);
                    HandleServerChatEvent(scArgs);
                    break;
            }

            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        private void HandleServerChatEvent(ServerChatEventArgs scArgs)
        {
            switch (scArgs.EventType)
            {
                case ChatEventType.Broadcast:
                    OnServerBroadcast(scArgs);
                    break;
                case ChatEventType.ChannelDNE:
                    OnChannelDidNotExist(scArgs);
                    break;
                case ChatEventType.ChannelFull:
                    OnChannelWasFull(scArgs);
                    break;
                case ChatEventType.ChannelRestricted:
                    OnChannelWasRestricted(scArgs);
                    break;
                case ChatEventType.Error:
                    OnServerErrorReceived(scArgs);
                    break;
                case ChatEventType.Information:
                    OnInformationReceived(scArgs);
                    break;
            }
        }

        private void HandleChatMessageEvent(ChatMessageEventArgs cmArgs)
        {
            switch (cmArgs.EventType)
            {
                case ChatEventType.Emote:
                    OnUserEmoted(cmArgs);
                    break;
                case ChatEventType.Talk:
                    OnUserSpoke(cmArgs);
                    break;
                case ChatEventType.WhisperReceived:
                    OnWhisperReceived(cmArgs);
                    break;
                case ChatEventType.WhisperSent:
                    OnWhisperSent(cmArgs);
                    break;
            } 
        }

        private void HandleUserChatEvent(UserEventArgs userEventArgs)
        {
            switch (userEventArgs.EventType)
            {
                case ChatEventType.UserFlagsChanged:
                    OnUserFlagsChanged(userEventArgs);
                    break;
                case ChatEventType.UserInChannel:
                    OnUserShown(userEventArgs);
                    break;
                case ChatEventType.UserJoinedChannel:
                    OnUserJoined(userEventArgs);
                    break;
                case ChatEventType.UserLeftChannel:
                    OnUserLeft(userEventArgs);
                    break;
            }
        }
        #endregion

        private void HandlePing(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            BncsPacket pck0x25 = new BncsPacket((byte)BncsPacketId.Ping);
            pck0x25.Insert(dr.ReadUInt32());
            // handles a special case that the 0x25 packet sometimes arrives before 0x50....
            if (m_received0x50)
            {
                Send(pck0x25);
                m_pingPck = null;
                m_received0x50 = false;
            }
            else
            {
                m_pingPck = pck0x25;
            }
            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        #region logon response
        private void HandleLogonResponse(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            bool success = dr.ReadBoolean();
            if (success)
            {
                OnLoginSucceeded(BaseEventArgs.GetEmpty(data));
                EnterChat();
            }
            else
            {
                OnLoginFailed(BaseEventArgs.GetEmpty(data));
                Close();
            }
        }

        private void HandleLogonResponse2(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            int success = dr.ReadInt32();
            if (success == 0)
            {
                OnLoginSucceeded(BaseEventArgs.GetEmpty(data));
                EnterChat();
            }
            else
            {
                OnLoginFailed(BaseEventArgs.GetEmpty(data));
            }
        }
        #endregion

        private void HandleWarcraftGeneral(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            WarcraftCommands cmd = (WarcraftCommands)dr.ReadByte();
            switch (cmd)
            {
                case WarcraftCommands.RequestLadderMap:
                    HandleWarcraftRequestLadderMap(data, dr);
                    break;
                default:
                    BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dr")]
        private void HandleWarcraftRequestLadderMap(ParseData data, DataReader dr)
        {
            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        #region client authentication
        private void HandleAuthInfo(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            if (m_pingPck != null)
            {
                Send(m_pingPck);
                m_pingPck = null;
            }
            m_received0x50 = true;

            m_loginType = dr.ReadUInt32();
            m_srvToken = dr.ReadUInt32();
            m_udpVal = dr.ReadUInt32();
            m_mpqFiletime = dr.ReadInt64();
            m_versioningFilename = dr.ReadCString();
            m_usingLockdown = m_versioningFilename.StartsWith("LOCKDOWN", StringComparison.OrdinalIgnoreCase);

            int crResult = -1, exeVer = -1;
            string exeInfo = null;

            if (!m_usingLockdown)
            {
                m_valString = dr.ReadCString();
                int mpqNum = CheckRevision.ExtractMPQNumber(m_versioningFilename);
                crResult = CheckRevision.DoCheckRevision(m_valString, new string[] { m_settings.GameExe, m_settings.GameFile2, m_settings.GameFile3 }, mpqNum);
                exeVer = CheckRevision.GetExeInfo(m_settings.GameExe, out exeInfo);
            }
            else
            {
                m_ldValStr = dr.ReadNullTerminatedByteArray();
                string dllName = m_versioningFilename.Replace(".mpq", ".dll");

                BnFtpVersion1Request req = new BnFtpVersion1Request(m_settings.Client, m_versioningFilename, null);
                req.Server = m_settings.Server;
                req.LocalFileName = Path.Combine(Path.GetTempPath(), m_versioningFilename);
                req.ExecuteRequest();

                string ldPath = null;
                using (MpqArchive arch = MpqServices.OpenArchive(req.LocalFileName))
                {
                    if (arch.ContainsFile(dllName))
                    {
                        ldPath = Path.Combine(Path.GetTempPath(), dllName);
                        arch.SaveToPath(dllName, Path.GetTempPath(), false);
                    }
                }

                m_ldDigest = CheckRevision.DoLockdownCheckRevision(m_ldValStr, new string[] { m_settings.GameExe, m_settings.GameFile2, m_settings.GameFile3 },
                                ldPath, m_settings.ImageFile, ref exeVer, ref crResult);
            }

            m_prodCode = m_settings.Client;

            if (m_prodCode == "WAR3" ||
                m_prodCode == "W3XP")
            {
                m_w3srv = dr.ReadByteArray(128);

                if (!NLS.ValidateServerSignature(m_w3srv, RemoteEP.Address.GetAddressBytes()))
                {
                    OnError(new ErrorEventArgs(Strings.War3ServerValidationFailed, true));
                    Close();
                    return;
                }
            }

            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);

            CdKey key1, key2 = null;
            key1 = new CdKey(m_settings.CdKey1);
            if (m_prodCode == "D2XP" || m_prodCode == "W3XP")
            {
                key2 = new CdKey(m_settings.CdKey2);
            }

            m_clientToken = unchecked((uint)new Random().Next());

            byte[] key1Hash = key1.GetHash(m_clientToken, m_srvToken);
            if (m_warden != null)
            {
                try
                {
                    if (m_warden.InitWarden(BitConverter.ToInt32(key1Hash, 0)))
                    {
                        m_warden.UninitWarden();
                        OnError(new ErrorEventArgs("The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                        m_warden = null;
                    }
                }
                catch (Win32Exception we)
                {
                    OnError(new ErrorEventArgs("The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                    OnError(new ErrorEventArgs(string.Format(CultureInfo.CurrentCulture, "Additional information: {0}", we.Message), false));
                    m_warden.UninitWarden();
                    m_warden = null;
                }
            }

            BncsPacket pck0x51 = new BncsPacket((byte)BncsPacketId.AuthCheck);
            pck0x51.Insert(m_clientToken);
            pck0x51.Insert(exeVer);
            pck0x51.Insert(crResult);
            if (m_prodCode == "D2XP" || m_prodCode == "W3XP")
                pck0x51.Insert(2);
            else
                pck0x51.Insert(1);
            pck0x51.Insert(false);
            pck0x51.Insert(key1.Key.Length);
            pck0x51.Insert(key1.Product);
            pck0x51.Insert(key1.Value1);
            pck0x51.Insert(0);
            pck0x51.Insert(key1Hash);
            if (key2 != null)
            {
                pck0x51.Insert(key2.Key.Length);
                pck0x51.Insert(key2.Product);
                pck0x51.Insert(key2.Value1);
                pck0x51.Insert(0);
                pck0x51.Insert(key2.GetHash(m_clientToken, m_srvToken));
            }

            if (m_usingLockdown)
            {
                pck0x51.InsertByteArray(m_ldDigest);
                pck0x51.InsertByte(0);
            }
            else
                pck0x51.InsertCString(exeInfo);

            pck0x51.InsertCString(m_settings.CdKeyOwner);

            Send(pck0x51);
        }

        private void HandleAuthCheck(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            uint result = dr.ReadUInt32();
            string extraInfo = dr.ReadCString();

            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);

            if (result == 0)
            {
                OnClientCheckPassed(BaseEventArgs.GetEmpty(null));
            }
            else
            {
                OnClientCheckFailed(new ClientCheckFailedEventArgs((ClientCheckFailureCause)result, extraInfo));
                Close();
                return;
            }

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

                case "WAR3":
                case "W3XP":
                    m_nls = new NLS(m_settings.Username, m_settings.Password);
                    BncsPacket pck0x53 = new BncsPacket((byte)BncsPacketId.AuthAccountLogon);
                    m_nls.LoginAccount(pck0x53);
                    Send(pck0x53);
                    break;
            }
        }

        private void HandleAuthAccountCreate(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            int status = dr.ReadInt32();
            switch (status)
            {
                case 0:
                    OnInformation(new InformationEventArgs(
                        string.Format(CultureInfo.CurrentCulture, Strings.NameCreateSuccess, m_settings.Username)));

                    BncsPacket pck0x53 = new BncsPacket((byte)BncsPacketId.AuthAccountLogon);
                    m_nls.LoginAccount(pck0x53);
                    Send(pck0x53);
                    break;
                case 7:
                    CloseWithError(Strings.NameCreateFail7);
                    break;
                case 8:
                    CloseWithError(Strings.NameCreateFail8);
                    break;
                case 9:
                    CloseWithError(Strings.NameCreateFail9);
                    break;
                case 10:
                    CloseWithError(Strings.NameCreateFail10);
                    break;
                case 11:
                    CloseWithError(Strings.NameCreateFail11);
                    break;
                case 12:
                    CloseWithError(Strings.NameCreateFail12);
                    break;
                case 4:
                default:
                    CloseWithError(Strings.NameCreateFailOther);
                    break;
            }
            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        private void HandleAuthAccountLogon(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);
            // 0x53
            // status codes:
            // 0: accepted, proof needed
            // 1: no such account
            // 5: upgrade account
            // else: unknown failure
            int status = dr.ReadInt32();
            //if (ApplicationEnvironment.IsDebugMode)
            //{
            //    m_owner.EventHost.OnError(m_owner, new ErrorEventArgs(
            //        string.Format("SID_AUTHACCOUNTLOGON response: 0x{0:x8}", status), null));
            //}

            if (status == 0)
            {
                byte[] salt = dr.ReadByteArray(32);
                byte[] serverKey = dr.ReadByteArray(32);

                BncsPacket pck0x54 = new BncsPacket((byte)BncsPacketId.AuthAccountLogonProof);
                m_nls.LoginProof(pck0x54, salt, serverKey);
                Send(pck0x54);
            }
            else
            {
                // create the account or error out.
                if (status == 1)
                {
                    OnInformation(new InformationEventArgs(
                        string.Format(CultureInfo.CurrentCulture, Strings.AccountDNECreating, m_settings.Username)));

                    BncsPacket pck0x52 = new BncsPacket((byte)BncsPacketId.AuthAccountCreate);
                    m_nls.CreateAccount(pck0x52);
                    Send(pck0x52);
                }
                else if (status == 5)
                {
                    OnError(new ErrorEventArgs(Strings.UpgradeRequestedUnsupported, true));
                    Close();
                    return;
                }
                else
                {
                    OnError(new ErrorEventArgs(Strings.InvalidUsernameOrPassword, true));
                    Close();
                    return;
                }
            }
        }

        private void HandleAuthAccountLogonProof(ParseData data)
        {
            DataReader dr = new DataReader(data.Data);

            int status = dr.ReadInt32();
            byte[] m2 = dr.ReadByteArray(20);
            // 0 = success
            // 2 = incorrect password
            // 14 = success; register e-mail
            // 15 = custom error
            switch (status)
            {
                case 0:
                    // success
                    if (!m_nls.VerifyServerProof(m2))
                    {
                        //CloseWithError(BattleNet.LoginProofServerProofFailed);
                        OnError(new ErrorEventArgs(Strings.LoginProofServerProofFailed, false));
                        EnterChat();
                    }
                    else
                    {
                        OnInformation(new InformationEventArgs(Strings.LoginProofSuccess));
                        OnLoginSucceeded(BaseEventArgs.GetEmpty(null));
                        EnterChat();
                    }
                    break;
                case 2:
                    CloseWithError(Strings.LoginProofClientProofFailed);
                    break;
                case 14:
                    if (!m_nls.VerifyServerProof(m2))
                    {
                        OnError(new ErrorEventArgs(Strings.LoginProofServerProofFailed, false));
                        EnterChat();
                    }
                    else
                    {
                        OnInformation(new InformationEventArgs(Strings.LoginProofRegisterEmail));
                        EnterChat();
                    }
                    break;
                case 15:
                    CloseWithError(
                        string.Format(CultureInfo.CurrentCulture, Strings.LoginProofCustomError, dr.ReadCString())
                    );
                    break;
            }

            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        #endregion

        private void EnterChat()
        {
            // this does two things.
            // in War3 and W3xp both string fields are null, but in older clients, the first string field is 
            // the username.  And, War3 and W3xp send the SID_NETGAMEPORT packet before entering chat, so we
            // send that packet, then insert the empty string into the ENTERCHAT packet.  We of course go to 
            // the other branch that inserts the username into the packet for older clients.
            // new for War3: it also sends a packet that seems to be required, 0x44 subcommand 2 (get ladder map info)
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.EnterChat);

            bool isClientWar3 = (m_settings.Client.Equals("WAR3", StringComparison.Ordinal) || m_settings.Client.Equals("W3XP", StringComparison.Ordinal));
            bool isClientStar = (m_settings.Client.Equals("STAR", StringComparison.Ordinal) || m_settings.Client.Equals("SEXP", StringComparison.Ordinal));
            if (isClientWar3)
            {
                BncsPacket pck0x45 = new BncsPacket((byte)BncsPacketId.NetGamePort);
                pck0x45.InsertInt16(6112);
                Send(pck0x45);

                BncsPacket pckGetLadder = new BncsPacket((byte)BncsPacketId.WarcraftGeneral);
                pckGetLadder.InsertByte((byte)WarcraftCommands.RequestLadderMap);
                pckGetLadder.InsertInt32(1); // cookie
                pckGetLadder.InsertByte(5); // number of types requested
                //pckGetLadder.InsertDwordString("URL");
                pckGetLadder.InsertInt32(0x004d4150);
                pckGetLadder.InsertInt32(0);
                //pckGetLadder.InsertDwordString("MAP");
                pckGetLadder.InsertInt32(0x0055524c);
                pckGetLadder.InsertInt32(0);
                pckGetLadder.InsertDwordString("TYPE");
                pckGetLadder.InsertInt32(0);
                pckGetLadder.InsertDwordString("DESC");
                pckGetLadder.InsertInt32(0);
                pckGetLadder.InsertDwordString("LADR");
                pckGetLadder.InsertInt32(0);
                Send(pckGetLadder);

                pck.InsertCString(string.Empty);
            }
            else
            {
                pck.InsertCString(m_settings.Username);
            }
            pck.InsertCString(string.Empty);
            Send(pck);

            if (!isClientWar3)
            {
                RequestChannelList();

                BncsPacket pckJoinChannel = new BncsPacket((byte)BncsPacketId.JoinChannel);
                string client = "Starcraft";
                switch (m_settings.Client)
                {
                    case "SEXP":
                        client = "Brood War";
                        break;
                    case "W2BN":
                        client = "Warcraft II BNE";
                        break;
                    case "D2DV":
                        client = "Diablo II";
                        break;
                    case "D2XP":
                        client = "Lord of Destruction";
                        break;
                }
                pckJoinChannel.InsertInt32((int)ChannelJoinFlags.FirstJoin);
                pckJoinChannel.InsertCString(client);
                Send(pckJoinChannel);
            }

            if (isClientWar3 || isClientStar)
            {
                pck = new BncsPacket((byte)BncsPacketId.FriendsList);
                Send(pck);
            }

            m_tmr.Start();
        }

        private void RequestChannelList()
        {
            BncsPacket pckChanReq = new BncsPacket((byte)BncsPacketId.GetChannelList);
            pckChanReq.InsertDwordString(m_settings.Client);
            Send(pckChanReq);
        }

        #region NLS password changing
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "data"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void HandleAuthAccountChange(ParseData data)
        {
            // TO DO
            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "data"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void HandleAuthAccountChangeProof(ParseData data)
        {
            // TO DO
            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }
        #endregion

        #region warden
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void HandleWarden(ParseData data)
        {
            if (m_warden != null)
            {
                byte[] total = new byte[4 + data.Length];
                total[0] = 0xff;
                total[1] = (byte)BncsPacketId.Warden;
                byte[] len = BitConverter.GetBytes(data.Length);
                total[2] = len[0];
                total[3] = len[1];

                Buffer.BlockCopy(data.Data, 0, total, 4, data.Length);

                m_warden.ProcessWarden(total);
            }
            else
            {
                OnWardentUnhandled(BaseEventArgs.GetEmpty(null));
            }

            BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);
        }
        #endregion

        #region utility methods
        partial void InitializeListenState()
        {
            m_packetQueue = new PriorityQueue<ParseData>();

            m_listener = new Thread(new ThreadStart(Listen));
            m_listener.IsBackground = true;
            m_listener.Name = "BN# Battle.net Listener Thread";

            m_parser = new Thread(new ThreadStart(Parse));
            m_parser.IsBackground = true;
            m_parser.Name = "BN# Battle.net Parser Thread";

            m_parseWait = new EventWaitHandle(false, EventResetMode.AutoReset);

            m_packetToPriorityMap = new Dictionary<BncsPacketId, Priority>();
            PopulateDefaultPacketPriorities();

            m_tmr = new System.Timers.Timer(60000.0);
            m_tmr.AutoReset = true;
            m_tmr.Elapsed += new System.Timers.ElapsedEventHandler(SendSidNull);

            m_news = new List<NewsEntry>();
        }

        void SendSidNull(object sender, System.Timers.ElapsedEventArgs e)
        {
            Send(new BncsPacket((byte)BncsPacketId.Null));
        }

        private Priority DeterminePriority(BncsPacketId bncsPacketId)
        {
            Priority p = Priority.Normal;
            if (m_packetToPriorityMap.ContainsKey(bncsPacketId))
                p = m_packetToPriorityMap[bncsPacketId];

            return p;
        }

        private void PopulateDefaultPacketPriorities()
        {
            BncsPacketId[] validIds = (BncsPacketId[])Enum.GetValues(typeof(BncsPacketId));
            foreach (BncsPacketId id in validIds)
            {
                m_packetToPriorityMap.Add(id, Priority.Normal);
            }
        }
        #endregion
        #region shortcuts
        private void CloseWithError(string p)
        {
            OnError(new ErrorEventArgs(p, true));
            Close();
        }
        #endregion

        private void StartParsing()
        {
            Debug.WriteLine("Starting the parsing thread.");
            m_parser.Start();
        }

        private void StopParsingAndListening()
        {
            m_parser.Abort();
            m_listener.Abort();
        }

        private void StartListening()
        {
            Debug.WriteLine("Starting the listening thread.");
            m_listener.Start();
        }
    }
}
