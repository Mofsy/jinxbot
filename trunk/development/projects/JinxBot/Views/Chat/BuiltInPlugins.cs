using BNSharp;
using BNSharp.BattleNet.Clans;
using BNSharp.BattleNet.Friends;
using JinxBot.Views.Chat;
using JinxBot.Plugins.UI;
using BNSharp.BattleNet;

[assembly: CustomListBoxRenderer(typeof(ClanMember), typeof(ClanListBoxItemRenderer))]
[assembly: CustomListBoxRenderer(typeof(ChatUser), typeof(ChannelListBoxItemRenderer))]
[assembly: CustomListBoxRenderer(typeof(FriendUser), typeof(FriendListBoxItemRenderer))]