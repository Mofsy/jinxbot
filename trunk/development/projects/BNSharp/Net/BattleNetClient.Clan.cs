using System;
using System.Collections.Generic;
using System.Text;
using BNSharp.BattleNet.Clans;
using BNSharp.MBNCSUtil;

namespace BNSharp.Net
{
    partial class BattleNetClient
    {
        private Dictionary<string, ClanMember> m_clanList = new Dictionary<string, ClanMember>();
        private string m_clanTag;
        private int m_clanCookie;

        partial void ResetClanState()
        {
            m_clanList.Clear();
            m_clanTag = null;
            m_clanCookie = 0;
        }

        /// <summary>
        /// If the client is logged on as a clan Chieftan or Shaman, sets the clan message-of-the-day.
        /// </summary>
        /// <param name="motd">The new message-of-the-day.</param>
        public void SetClanMessageOfTheDay(string motd)
        {
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanSetMOTD);
            pck.InsertInt32(m_clanCookie++);
            pck.InsertCString(motd);

            Send(pck);
        }

        private void HandleClanInfo(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(1);
            string clanTag = dr.ReadDwordString(0);
            ClanRank rank = (ClanRank)dr.ReadByte();

            ClanMembershipEventArgs args = new ClanMembershipEventArgs(clanTag, rank);
            args.EventData = pd;
            OnClanMembershipReceived(args);

            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanMemberList);
            pck.InsertInt32(m_clanCookie++);
            Send(pck);
        }

        /// <summary>
        /// Begins searching for clan candidates in the channel and friends list, and checks the availability of the specified clan tag.
        /// </summary>
        /// <param name="clanTag">The clan tag to check for availability.</param>
        /// <remarks>
        /// <para>This method will return immediately, but will cause the <see>ClanCandidatesSearchCompleted</see> event to be fired.  That event does not
        /// specifically indicate that the proper number of candidates were found, simply that Battle.net responded.  The event arguments sent
        /// as part of the event indicate the success or failure of the request.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="clanTag"/> is <see langword="null" />.</exception>
        public void BeginClanCandidatesSearch(string clanTag)
        {
            if (object.ReferenceEquals(clanTag, null))
                throw new ArgumentNullException("clanTag");

            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanFindCandidates);
            pck.InsertInt32(m_clanCookie++);
            pck.InsertDwordString(clanTag, 0);

            Send(pck);
        }

        /// <summary>
        /// Invites the specified number of users to form a new clan.
        /// </summary>
        /// <param name="clanName">The name of the clan to form.</param>
        /// <param name="clanTag">The tag of the clan to form.</param>
        /// <param name="usersToInvite">The list of users to invite.  This parameter must be exactly 9 items long.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="clanName"/>, <paramref name="clanTag"/>, 
        /// <paramref name="userToInvite"/>, or any of the strings in the array of <paramref name="usersToInvite"/>
        /// is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="usersToInvite"/> is not exactly 9 items long.</exception>
        public void InviteUsersToNewClan(string clanName, string clanTag, string[] usersToInvite)
        {
            if (object.ReferenceEquals(null, clanName)) throw new ArgumentNullException("clanName");
            if (object.ReferenceEquals(null, clanTag)) throw new ArgumentNullException("clanTag");
            if (object.ReferenceEquals(null, usersToInvite)) throw new ArgumentNullException("usersToInvite");
            if (usersToInvite.Length != 9) throw new ArgumentOutOfRangeException("usersToInvite", usersToInvite, "Exactly 9 users may be invited to form a new clan.");
            for (int i = 0; i < 9; i++)
                if (object.ReferenceEquals(usersToInvite[i], null)) throw new ArgumentNullException("usersToInvite", "One or more names of users to invite is null.");

            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanInviteMultiple);
            pck.InsertInt32(m_clanCookie++);
            pck.InsertCString(clanName);
            pck.InsertDwordString(clanTag, 0);
            pck.InsertByte(9);
            for (int i = 0; i < 9; i++)
            {
                pck.InsertCString(usersToInvite[i]);
            }

            Send(pck);
        }

        // 0x71
        private void HandleClanInviteMultiple(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(4); // cookie
            ClanResponseCode response = (ClanResponseCode)dr.ReadByte();
            ClanFormationEventArgs args = null;
            if (response == ClanResponseCode.Success)
            {
                args = new ClanFormationEventArgs();
            }
            else
            {
                List<string> names = new List<string>();
                int nextByte = dr.Peek();
                while (nextByte > 0)
                {
                    names.Add(dr.ReadCString());
                    nextByte = dr.Peek();
                }
                args = new ClanFormationEventArgs(response == ClanResponseCode.InvitationDeclined, response == ClanResponseCode.Decline, names.ToArray());
            }
            args.EventData = pd;

            OnClanFormationCompleted(args);
        }

        // 0x72
        private void HandleClanCreationInvitation(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            int cookie = dr.ReadInt32();
            string tag = dr.ReadDwordString(0);
            string name = dr.ReadCString();
            string inviter = dr.ReadCString();
            int inviteeCount = dr.ReadByte();
            string[] invitees = new string[inviteeCount];
            for (int i = 0; i < inviteeCount; i++)
            {
                invitees[i] = dr.ReadCString();
            }

            ClanFormationInvitationEventArgs args = new ClanFormationInvitationEventArgs(cookie, tag, name, inviter, invitees) { EventData = pd };
            OnClanFormationInvitationReceived(args);
        }

        /// <summary>
        /// Responds to the invitation to form a new clan.
        /// </summary>
        /// <param name="requestID">The request ID, provided by the <see cref="ClanFormationInvitationEventArgs.RequestID">ClanFormationInvitationEventArgs</see>.</param>
        /// <param name="clanTag">The clan tag.</param>
        /// <param name="inviter">The user who invited the client to the clan.</param>
        /// <param name="accept">Whether to accept the invitation.</param>
        public void RespondToNewClanInvitation(int requestID, string clanTag, string inviter, bool accept)
        {
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanCreationInvitation);
            pck.InsertInt32(requestID);
            pck.InsertDwordString(clanTag, 0);
            pck.InsertCString(inviter);
            pck.InsertByte((byte)(accept ? ClanResponseCode.Accept : ClanResponseCode.Decline));

            Send(pck);
        }

        /// <summary>
        /// Disbands the clan to which the client belongs.
        /// </summary>
        /// <remarks>
        /// <para>The client must be the leader of the clan in order to send this command.</para>
        /// </remarks>
        public void DisbandClan()
        {
            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanDisband);
            pck.InsertInt32(m_clanCookie++);
            Send(pck);
        }

        // 0x73
        private void HandleDisbandClan(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(4);
            ClanResponseCode status = (ClanResponseCode)dr.ReadByte();
            ClanDisbandEventArgs args = new ClanDisbandEventArgs(status == ClanResponseCode.Success);
            args.EventData = pd;
            OnClanDisbandCompleted(args);
        }

        /// <summary>
        /// Designates a user as a new clan chieftan (leader).
        /// </summary>
        /// <param name="newChieftanName">The name of the new clan chieftan.</param>
        public void DesignateClanChieftan(string newChieftanName)
        {
            if (object.ReferenceEquals(null, newChieftanName))
                throw new ArgumentNullException("newChieftanName");

            BncsPacket pck = new BncsPacket((byte)BncsPacketId.ClanMakeChieftan);
            pck.InsertInt32(m_clanCookie++);
            pck.InsertCString(newChieftanName);

            Send(pck);
        }

        private void HandleClanMakeChieftan(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(4);
            ClanChieftanChangeResult result = (ClanChieftanChangeResult)dr.ReadByte();
            ClanChieftanChangeEventArgs args = new ClanChieftanChangeEventArgs(result);
            args.EventData = pd;
            OnClanChangeChieftanCompleted(args);
        }

        private void HandleClanQuitNotify(ParseData pd)
        {
            
        }

        private void HandleClanInvitation(ParseData pd)
        {

        }

        private void HandleClanRemoveMember(ParseData pd)
        {

        }

        private void HandleClanInvitationResponse(ParseData pd)
        {

        }

        private void HandleClanFindCandidates(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(4); // skip the cookie
            ClanCandidatesSearchStatus status = (ClanCandidatesSearchStatus)dr.ReadByte();
            int numCandidates = dr.ReadByte();
            string[] usernames = new string[numCandidates];
            for (int i = 0; i < numCandidates; i++)
            {
                usernames[i] = dr.ReadCString();
            }

            ClanCandidatesSearchEventArgs args = new ClanCandidatesSearchEventArgs(status, usernames);
            args.EventData = pd;
            OnClanCandidatesSearchCompleted(args);
        }

        private void HandleClanRankChange(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            ClanRank old = (ClanRank)dr.ReadByte();
            ClanRank newRank = (ClanRank)dr.ReadByte();
            string memberName = dr.ReadCString();
            ClanMember member = null;
            if (m_clanList.ContainsKey(memberName))
                member = m_clanList[memberName];

            ClanRankChangeEventArgs args = new ClanRankChangeEventArgs(old, newRank, member);
            args.EventData = pd;
            OnClanRankChanged(args);
        }

        private void HandleClanMotd(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(8);
            string motd = dr.ReadCString();
            InformationEventArgs args = new InformationEventArgs(motd);
            args.EventData = pd;
            OnClanMessageOfTheDay(args);
        }

        private void HandleClanMemberList(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            dr.Seek(4);
            byte memCount = dr.ReadByte();
            for (int i = 0; i < memCount; i++)
            {
                string userName = dr.ReadCString();
                ClanRank rank = (ClanRank)dr.ReadByte();
                ClanMemberStatus status = (ClanMemberStatus)dr.ReadByte();
                string location = dr.ReadCString();
                m_clanList.Add(userName, new ClanMember(userName, rank, status, location));

            }

            ClanMember[] members = new ClanMember[m_clanList.Count];
            m_clanList.Values.CopyTo(members, 0);

            ClanMemberListEventArgs args = new ClanMemberListEventArgs(members);
            args.EventData = pd;
            OnClanMemberListReceived(args);
        }

        private void HandleClanMemberRemoved(ParseData pd)
        {

        }

        private void HandleClanMemberStatusChanged(ParseData pd)
        {
            DataReader dr = new DataReader(pd.Data);
            string userName = dr.ReadCString();
            if (m_clanList.ContainsKey(userName))
            {
                ClanMember member = m_clanList[userName];
                ClanRank rank = (ClanRank)dr.ReadByte();
                ClanMemberStatus status = (ClanMemberStatus)dr.ReadByte();
                string location = dr.ReadCString();
                member.Rank = rank;
                member.CurrentStatus = status;
                member.Location = location;

                ClanMemberStatusEventArgs args = new ClanMemberStatusEventArgs(member);
                args.EventData = pd;
                OnClanMemberStatusChanged(args);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pd"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ClanMemberRankChanged(ParseData pd)
        {

        }

        private void HandleClanMemberInformation(ParseData pd)
        {

        }
    }
}
