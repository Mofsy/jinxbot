using BNSharp.BattleNet.Core;
using BNSharp.BattleNet.Ftp;
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

        private void ResetState()
        {
            _received0x50 = false;
            _loginType = _udpVal = 0;
            _clientToken = _srvToken = 0;
            _mpqFileTime = 0;
            _versioningFilename = null;
            _usingLockdown = false;
            _ldValStr = null;
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
            }
        }

        private void HandleAuthCheck(BncsReader packet)
        {
            //uint result = packet.ReadUInt32();
            //string extraInfo = packet.ReadCString();

            //if (result == 0)
            //{
            //    OnClientCheckPassed(BaseEventArgs.GetEmpty(null));
            //}
            //else
            //{
            //    OnClientCheckFailed(new ClientCheckFailedEventArgs((ClientCheckFailureCause)result, extraInfo));
            //    Close();
            //    return;
            //}

            //ContinueLogin();
        }
    }
}
