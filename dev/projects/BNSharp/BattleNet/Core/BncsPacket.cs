using BNSharp.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Core
{
    /// <summary>
    /// Completes a <see cref="DataBuffer">DataBuffer</see> implementation with the additional
    /// data used by the BNCS protocol.
    /// </summary>
    /// <example>
    /// <para>The following example illustrates creating a buffer that contains only the
    /// <b>SID_NULL</b> packet:</para>
    /// <para><i><b>Note</b>: this example assumes you have a 
    /// <see cref="System.Net.Sockets.Socket">Socket</see> object called "sck" in the 
    /// current context</i>.</para>
    /// <code language="C#">
    /// [C#]
    /// BncsPacket pckNull = new BncsPacket(0);
    /// sck.Send(pckNull.GetData(), 0, pckNull.Count, SocketFlags.None);
    /// </code>
    /// <code language="Visual Basic">
    /// [Visual Basic]
    /// Dim pckNull As New BncsPacket(0)
    /// sck.Send(pckNull.GetData(), 0, pckNull.Count, SocketFlags.None)
    /// </code>
    /// <code language="C++">
    /// [C++]
    /// BncsPacket ^pckNull = gcnew BncsPacket(0);
    /// sck->Send(pckNull->GetData(), 0, pckNull->Count, SocketFlags.None);
    /// </code>
    /// </example>
    /// <example>
    /// <para>The following example illustrates calculating the revision check (SID_AUTH_ACCONTLOGON) of Warcraft III:</para>
    /// <para><i><b>Note</b>: this example assumes you have a 
    /// <see cref="System.Net.Sockets.Socket">Socket</see> object called "sck" in the 
    /// current context</i>.</para>
    /// <code language="C#">
    /// [C#]
    /// BncsPacket pckLogin = new BncsPacket(0x53);
    /// NLS nls = new NLS(userName, password);
    /// nls.LoginAccount(pckLogin);
    /// sck.Send(pckLogin.GetData(), 0, pckLogin.Count, SocketFlags.None);
    /// </code>
    /// <code language="Visual Basic">
    /// [Visual Basic]
    /// Dim pckLogin As New BncsPacket(&amp;H51)
    /// Dim nls As New NLS(userName, password)
    /// nls.LoginAccount(pckLogin)
    /// sck.Send(pckLogin.GetData(), 0, pckLogin.Count, SocketFlags.None)
    /// </code>
    /// <code language="C++">
    /// [C++]
    /// // NOTE that userName and password must be System::String^, not char*!
    /// BncsPacket ^pckLogin = gcnew BncsPacket(0x51);
    /// NLS ^nls = gcnew NLS(userName, password);
    /// nls->LoginAccount(pckLogin)
    /// sck->Send(pckLogin->GetData(), 0, pckLogin->Count, SocketFlags.None);
    /// </code>
    /// </example>
    public sealed class BncsPacket : DataBuffer
    {
        private byte _id;

        /// <summary>
        /// Creates a new BNCS packet with the specified packet ID.
        /// </summary>
        /// <param name="id">The BNCS packet ID.</param>
        public BncsPacket(BncsPacketId id, NetworkBuffer backingStore)
            : base(backingStore)
        {
            _id = (byte)id;
            InsertByte(0xff);
            InsertByte((byte)id);
            InsertInt16(0);
        }

        /// <summary>
        /// Finalizes the data in the packet before preparing to send.
        /// </summary>
        public void ConstructPacket()
        {
            NetworkBuffer buffer = base.UnderlyingBuffer;
            byte[] tmp = buffer.UnderlyingBuffer;
            byte[] len = BitConverter.GetBytes((ushort)Count);
            tmp[2] = len[0];
            tmp[3] = len[1];
        }

        /// <summary>
        /// Gets or sets the ID of the packet as it was specified when it was created.
        /// </summary>
        public BncsPacketId PacketID
        {
            get { return (BncsPacketId)_id; }
            set { _id = (byte)value; }
        }


        public override Task<int> SendAsync(AsyncConnectionBase connection)
        {
            ConstructPacket();
            return base.SendAsync(connection);
        }
    }
}
