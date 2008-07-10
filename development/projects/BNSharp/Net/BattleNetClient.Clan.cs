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
