using BNSharp.BattleNet.Core;
using BNSharp.BattleNet.Ftp;
using BNSharp.Chat;
using BNSharp.Networking;
using BNSharp.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    partial class BattleNetClient
    {
        private bool _received0x50;
        private uint _loginType, _udpVal;
        private int _clientToken, _srvToken;
        private long _mpqFileTime;
        private string _versioningFilename, _valString;
        private bool _usingLockdown;
        private byte[] _ldValStr, _ldDigest, _w3srv;
        private NLS _nls;
        private bool _firstChannelList;

        private BncsPacket CreatePacket(BncsPacketId id)
        {
            return new BncsPacket(id, _connection.NetworkBuffers.Acquire());
        }

        private void ResetState()
        {
            _received0x50 = false;
            _loginType = _udpVal = 0;
            _clientToken = _srvToken = 0;
            _mpqFileTime = 0;
            _versioningFilename = null;
            _usingLockdown = false;
            _ldValStr = null;
            _firstChannelList = false;
        }

        private void HandleChatEvent(BncsReader dr)
        {

        }

        private async void HandleAuthInfo(BncsReader dr)
        {
            try
            {
                //DataReader dr = new DataReader(data.Data);
                //if (m_pingPck != null)
                //{
                //    Send(m_pingPck);
                //    m_pingPck = null;
                //}
                _received0x50 = true;

                _loginType = dr.ReadUInt32();
                _srvToken = dr.ReadInt32();
                _udpVal = dr.ReadUInt32();
                _mpqFileTime = dr.ReadInt64();
                _versioningFilename = dr.ReadCString();
                _usingLockdown = _versioningFilename.StartsWith("LOCKDOWN", StringComparison.OrdinalIgnoreCase);

                int crResult = -1, exeVer = -1;
                string exeInfo = null;

                if (!_usingLockdown)
                {
                    _valString = dr.ReadCString();
                    int mpqNum = CheckRevision.ExtractMPQNumber(_versioningFilename);
                    crResult = CheckRevision.DoCheckRevision(_valString, new Stream[] { File.OpenRead(_settings.GameExe), File.OpenRead(_settings.GameFile2), File.OpenRead(_settings.GameFile3) }, mpqNum);
                    exeVer = CheckRevision.GetExeInfo(_settings.GameExe, out exeInfo);
                }
                else
                {
                    _ldValStr = dr.ReadNullTerminatedByteArray();
                    string dllName = _versioningFilename.Replace(".mpq", ".dll");

                    BnFtpVersion1Request req = new BnFtpVersion1Request(_settings.Client, _versioningFilename, null);
                    req.Gateway = _settings.Gateway;
                    req.LocalFileName = Path.Combine(Path.GetTempPath(), _versioningFilename);
                    await req.ExecuteRequest();

                    string ldPath = null;
                    using (MpqArchive arch = MpqServices.OpenArchive(req.LocalFileName))
                    {
                        if (arch.ContainsFile(dllName))
                        {
                            ldPath = Path.Combine(Path.GetTempPath(), dllName);
                            arch.SaveToPath(dllName, Path.GetTempPath(), false);
                        }
                    }

                    _ldDigest = CheckRevision.DoLockdownCheckRevision(_ldValStr, new string[] { _settings.GameExe, _settings.GameFile2, _settings.GameFile3 },
                                    ldPath, _settings.ImageFile, ref exeVer, ref crResult);
                }

                string prodCode = _settings.Client.ProductCode;

                if (prodCode == "WAR3" || prodCode == "W3XP")
                {
                    _w3srv = dr.ReadByteArray(128);

                    if (!NLS.ValidateServerSignature(_w3srv, _connection.RemoteEP.Address.GetAddressBytes()))
                    {
                        //OnError(new ErrorEventArgs(ErrorType.Warcraft3ServerValidationFailure, Strings.War3ServerValidationFailed, false));
                        //Close();
                        //return;
                    }
                }

            //    BattleNetClientResources.IncomingBufferPool.FreeBuffer(data.Data);

                CdKey key1 = _settings.CdKey1, key2 = _settings.CdKey2;
                _clientToken = new Random().Next();

                byte[] key1Hash = key1.GetHash(_clientToken, _srvToken);
                if (WardenHandler != null)
                {
                    try
                    {
                        if (!WardenHandler.InitWarden(BitConverter.ToInt32(key1Hash, 0)))
                        {
                            WardenHandler.UninitWarden();
                            //OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, "The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                            WardenHandler = null;
                        }
                    }
                    catch (Win32Exception we)
                    {
                        //OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, "The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                        //OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, string.Format(CultureInfo.CurrentCulture, "Additional information: {0}", we.Message), false));
                        WardenHandler.UninitWarden();
                        WardenHandler = null;
                    }
                }

                BncsPacket pck0x51 = new BncsPacket(BncsPacketId.AuthCheck, _connection.NetworkBuffers.Acquire());
                pck0x51.InsertInt32(_clientToken);
                pck0x51.InsertInt32(exeVer);
                pck0x51.InsertInt32(crResult);
                if (prodCode == "D2XP" || prodCode == "W3XP")
                    pck0x51.InsertInt32(2);
                else
                    pck0x51.InsertInt32(1);
                pck0x51.InsertBoolean(false);
                pck0x51.InsertInt32(key1.Key.Length);
                pck0x51.InsertInt32(key1.Product);
                pck0x51.InsertInt32(key1.Value1);
                pck0x51.InsertInt32(0);
                pck0x51.InsertByteArray(key1Hash);
                if (key2 != null)
                {
                    pck0x51.InsertInt32(key2.Key.Length);
                    pck0x51.InsertInt32(key2.Product);
                    pck0x51.InsertInt32(key2.Value1);
                    pck0x51.InsertInt32(0);
                    pck0x51.InsertByteArray(key2.GetHash(_clientToken, _srvToken));
                }

                if (_usingLockdown)
                {
                    pck0x51.InsertByteArray(_ldDigest);
                    pck0x51.InsertByte(0);
                }
                else
                    pck0x51.InsertCString(exeInfo);

                pck0x51.InsertCString(_settings.CdKeyOwner);

                await pck0x51.SendAsync(_connection);
            }
            catch (Exception ex)
            {
                //OnError(new ErrorEventArgs(ErrorType.General, "There was an error while initializing your client.  Refer to the exception message for more information.\n" + ex.ToString(), true));
                Disconnect();

                Console.WriteLine(ex.ToString());
            }
        }

        private void HandleAuthCheck(BncsReader packet)
        {
            uint result = packet.ReadUInt32();
            string extraInfo = packet.ReadCString();

            if (result == 0)
            {
                OnClientCheckPassed();
            }
            else
            {
                OnClientCheckFailed(new ClientCheckFailedEventArgs((ClientCheckFailureCause)result, extraInfo));
                Disconnect();
                return;
            }

            ContinueLogin();
        }

        private async void HandlePing(BncsReader packet)
        {
            int ping = packet.ReadInt32();

            NetworkBuffer buffer = _storage.Acquire();
            BncsPacket response = new BncsPacket(BncsPacketId.Ping, buffer);
            response.InsertInt32(ping);

            await response.SendAsync(_connection);
        }

        #region CreateAccount implementations
        private async void CreateAccountOld()
        {
            byte[] passwordHash = OldAuth.HashPassword(_settings.Password);
            BncsPacket pck = new BncsPacket(BncsPacketId.CreateAccount2, _storage.Acquire());
            pck.InsertByteArray(passwordHash);
            pck.InsertCString(_settings.Username);

            await pck.SendAsync(_connection);
        }

        private async void CreateAccountNLS()
        {
            BncsPacket pck = new BncsPacket(BncsPacketId.AuthAccountCreate, _storage.Acquire());
            _nls = new NLS(_settings.Username, _settings.Password);
            _nls.CreateAccount(pck);

            await pck.SendAsync(_connection);
        }

        private void HandleCreateAccount2(BncsReader dr)
        {
            int status = dr.ReadInt32();
            CreationFailureReason reason = CreationFailureReason.Unknown;
            switch (status)
            {
                case 2:
                    reason = CreationFailureReason.InvalidCharacters; break;
                case 3:
                    reason = CreationFailureReason.InvalidWord; break;
                case 6:
                    reason = CreationFailureReason.NotEnoughAlphanumerics; break;
                case 4:
                default:
                    reason = CreationFailureReason.AccountAlreadyExists; break;
            }

            if (status == 0)
            {
                AccountCreationEventArgs created = new AccountCreationEventArgs(_settings.Username);
                OnAccountCreated(created);

                LoginAccountOld();
            }
            else
            {
                AccountCreationFailedEventArgs failed = new AccountCreationFailedEventArgs(_settings.Username, reason);
                OnAccountCreationFailed(failed);
            }
        }
        #endregion
        #region LoginAccount implementations

        private async void LoginAccountNLS()
        {
            _nls = new NLS(_settings.Username, _settings.Password);

            BncsPacket pck0x53 = new BncsPacket(BncsPacketId.AuthAccountLogon, _storage.Acquire());
            _nls.LoginAccount(pck0x53);
            await pck0x53.SendAsync(_connection);
        }

        private async void LoginAccountOld()
        {
            switch (_settings.Client.ProductCode)
            {
                case "W2BN":
                    BncsPacket pck0x29 = new BncsPacket(BncsPacketId.LogonResponse, _storage.Acquire());
                    pck0x29.InsertInt32(_clientToken);
                    pck0x29.InsertInt32(_srvToken);
                    pck0x29.InsertByteArray(OldAuth.DoubleHashPassword(_settings.Password, _clientToken, _srvToken));
                    pck0x29.InsertCString(_settings.Username);

                    await pck0x29.SendAsync(_connection);
                    break;
                case "STAR":
                case "SEXP":
                case "D2DV":
                case "D2XP":
                    BncsPacket pck0x3a = new BncsPacket(BncsPacketId.LogonResponse2, _storage.Acquire());
                    pck0x3a.InsertInt32(_clientToken);
                    pck0x3a.InsertInt32(_srvToken);
                    pck0x3a.InsertByteArray(OldAuth.DoubleHashPassword(_settings.Password, _clientToken, _srvToken));
                    pck0x3a.InsertCString(_settings.Username);

                    await pck0x3a.SendAsync(_connection);
                    break;

                default:
                    throw new NotSupportedException(string.Format("Client '{0}' is not supported with old-style account login.", _settings.Client.ProductCode));
            }
        }
        #endregion

        private async void HandleLogonResponse(BncsReader dr)
        {
            int status = dr.ReadInt32();
            if (status == 1)
            {
                OnLoginSucceeded();
                ClassicProduct product = _settings.Client;
                if (product.UsesUdpPing)
                {
                    BncsPacket pck = new BncsPacket(BncsPacketId.UdpPingResponse, _storage.Acquire());
                    pck.InsertDwordString("bnet");
                    await pck.SendAsync(_connection);
                }

                EnterChat();
            }
            else
            {
                LoginFailedEventArgs args = new LoginFailedEventArgs(LoginFailureReason.InvalidAccountOrPassword, status);
                OnLoginFailed(args);
            }
        }

        private async void HandleLogonResponse2(BncsReader dr)
        {
            int success = dr.ReadInt32();
            if (success == 0)
            {
                OnLoginSucceeded();
                ClassicProduct product = _settings.Client;
                if (product.UsesUdpPing)
                {
                    BncsPacket pck = new BncsPacket(BncsPacketId.UdpPingResponse, _storage.Acquire());
                    pck.InsertDwordString("bnet");
                    await pck.SendAsync(_connection);
                }

                EnterChat();
            }
            else
            {
                LoginFailureReason reason = LoginFailureReason.Unknown;
                switch (success)
                {
                    case 1: // account DNE
                        reason = LoginFailureReason.AccountDoesNotExist; break;
                    case 2: // invalid password
                        reason = LoginFailureReason.InvalidAccountOrPassword; break;
                    case 6: // account closed
                        reason = LoginFailureReason.AccountClosed; break;
                }
                LoginFailedEventArgs args = new LoginFailedEventArgs(reason, success, dr.ReadCString());
                OnLoginFailed(args);
            }
        }

        private async void EnterChat()
        {
            // this does two things.
            // in War3 and W3xp both string fields are null, but in older clients, the first string field is 
            // the username.  And, War3 and W3xp send the SID_NETGAMEPORT packet before entering chat, so we
            // send that packet, then insert the empty string into the ENTERCHAT packet.  We of course go to 
            // the other branch that inserts the username into the packet for older clients.
            // new for War3: it also sends a packet that seems to be required, 0x44 subcommand 2 (get ladder map info)
            BncsPacket pck = new BncsPacket(BncsPacketId.EnterChat, _storage.Acquire());

            bool isClientWar3 = (_settings.Client.Equals(ClassicProduct.Warcraft3Retail) || _settings.Client.Equals(ClassicProduct.Warcraft3Expansion));
            bool isClientStar = (_settings.Client.Equals(ClassicProduct.StarcraftRetail) || _settings.Client.Equals(ClassicProduct.StarcraftBroodWar));
            if (isClientWar3)
            {
                BncsPacket pck0x45 = new BncsPacket(BncsPacketId.NetGamePort, _storage.Acquire());
                pck0x45.InsertInt16(6112);
                await pck0x45.SendAsync(_connection);

                BncsPacket pckGetLadder = new BncsPacket(BncsPacketId.WarcraftGeneral, _storage.Acquire());
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
                await pckGetLadder.SendAsync(_connection);

                pck.InsertCString(string.Empty);
            }
            else
            {
                pck.InsertCString(_settings.Username);
            }
            pck.InsertCString(string.Empty);
            await pck.SendAsync(_connection);

            if (!isClientWar3)
            {
                RequestChannelList();

                //BncsPacket pckJoinChannel = new BncsPacket(BncsPacketId.JoinChannel, _storage.Acquire());
                //string client = "Starcraft";
                //switch (_settings.Client.ProductCode)
                //{
                //    case "SEXP":
                //        client = "Brood War";
                //        break;
                //    case "W2BN":
                //        client = "Warcraft II BNE";
                //        break;
                //    case "D2DV":
                //        client = "Diablo II";
                //        break;
                //    case "D2XP":
                //        client = "Lord of Destruction";
                //        break;
                //}
                //pckJoinChannel.InsertInt32((int)ChannelJoinFlags.FirstJoin);
                //pckJoinChannel.InsertCString(client);
                //await pckJoinChannel.SendAsync(_connection);
            }

            if (isClientWar3 || isClientStar)
            {
                pck = new BncsPacket(BncsPacketId.FriendsList, _storage.Acquire());
                await pck.SendAsync(_connection);
            }

            //m_tmr.Start();
        }

        private async void RequestChannelList()
        {
            BncsPacket pckChanReq = new BncsPacket(BncsPacketId.GetChannelList, _storage.Acquire());
            pckChanReq.InsertDwordString(_settings.Client.ProductCode);
            await pckChanReq.SendAsync(_connection);
        }

        private async void HandleGetChannelList(BncsReader dr)
        {
            List<string> channelList = new List<string>();
            string channel;
            do
            {
                channel = dr.ReadCString();
                if (!string.IsNullOrEmpty(channel))
                    channelList.Add(channel);
            } while (!string.IsNullOrEmpty(channel));

            ChannelListEventArgs e = new ChannelListEventArgs(channelList.ToArray());
            ((IChatConnectionEventSource)this).OnChannelListReceived(e);

            if (!_firstChannelList)
            {
                _firstChannelList = true;

                BncsPacket pckJoinChan = CreatePacket(BncsPacketId.JoinChannel);
                if (_settings.Client == ClassicProduct.Diablo2Retail || _settings.Client == ClassicProduct.Diablo2Expansion)
                    pckJoinChan.InsertInt32((int)ChannelJoinFlags.Diablo2FirstJoin);
                else
                    pckJoinChan.InsertInt32((int)ChannelJoinFlags.FirstJoin);

                switch (_settings.Client.ProductCode)
                {
                    case "STAR":
                    case "SEXP":
                    case "W2BN":
                    case "D2DV":
                    case "D2XP":
                    case "JSTR":
                        pckJoinChan.InsertCString(_settings.Client.ProductCode);
                        break;
                    case "WAR3":
                    case "W3XP":
                        pckJoinChan.InsertCString("W3");
                        break;
                }

                await pckJoinChan.SendAsync(_connection);
            }
        }
    }
}
