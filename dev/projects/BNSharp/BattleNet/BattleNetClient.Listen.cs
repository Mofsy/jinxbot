using BNSharp.BattleNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet
{
    partial class BattleNetClient
    {
        private bool _received0x50;
        private uint _loginType, _srvToken, _udpVal;
        private long _mpqFileTime;
        private string _versioningFilename, _valString;
        private bool _usingLockdown;

        private void ResetState()
        {
            _received0x50 = false;
            _loginType = _srvToken = _udpVal = 0;
            _mpqFileTime = 0;
            _versioningFilename = null;
            _usingLockdown = false;
        }

        private void HandleAuthInfo(BncsReader data)
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

                _loginType = data.ReadUInt32();
                _srvToken = data.ReadUInt32();
                _udpVal = data.ReadUInt32();
                _mpqFiletime = data.ReadInt64();
                _versioningFilename = data.ReadCString();
                _usingLockdown = _versioningFilename.StartsWith("LOCKDOWN", StringComparison.OrdinalIgnoreCase);

                int crResult = -1, exeVer = -1;
                string exeInfo = null;

                if (!_usingLockdown)
                {
                    _valString = data.ReadCString();
                    int mpqNum = CheckRevision.ExtractMPQNumber(_versioningFilename);
                    crResult = CheckRevision.DoCheckRevision(_valString, new string[] { _settings.GameExe, _settings.GameFile2, _settings.GameFile3 }, mpqNum);
                    exeVer = CheckRevision.GetExeInfo(m_settings.GameExe, out exeInfo);
                }
                else
                {
                    m_ldValStr = dr.ReadNullTerminatedByteArray();
                    string dllName = m_versioningFilename.Replace(".mpq", ".dll");

                    BnFtpVersion1Request req = new BnFtpVersion1Request(m_settings.Client, m_versioningFilename, null);
                    req.Server = m_settings.Gateway.ServerHost;
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
                        OnError(new ErrorEventArgs(ErrorType.Warcraft3ServerValidationFailure, Strings.War3ServerValidationFailed, false));
                        //Close();
                        //return;
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
                        if (!m_warden.InitWarden(BitConverter.ToInt32(key1Hash, 0)))
                        {
                            m_warden.UninitWarden();
                            OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, "The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                            m_warden = null;
                        }
                    }
                    catch (Win32Exception we)
                    {
                        OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, "The Warden module failed to initialize.  You will not be immediately disconnected; however, you may be disconnected after a short period of time.", false));
                        OnError(new ErrorEventArgs(ErrorType.WardenModuleFailure, string.Format(CultureInfo.CurrentCulture, "Additional information: {0}", we.Message), false));
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
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ErrorType.General, "There was an error while initializing your client.  Refer to the exception message for more information.\n" + ex.ToString(), true));
                Close();
            }
        }

        private void HandleAuthCheck(BncsReader packet)
        {
            uint result = packet.ReadUInt32();
            string extraInfo = packet.ReadCString();

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

            ContinueLogin();
        }
    }
}
