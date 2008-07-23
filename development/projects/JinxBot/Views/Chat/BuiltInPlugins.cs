using BNSharp;
using BNSharp.BattleNet.Clans;
using BNSharp.BattleNet.Friends;
using JinxBot.Views.Chat;
using JinxBot.Plugins.UI;

[assembly: CustomListBoxRenderer(typeof(ClanMember), typeof(ClanListBoxItemRenderer))]
[assembly: CustomListBoxRenderer(typeof(UserEventArgs), typeof(ChannelListBoxItemRenderer))]
[assembly: CustomListBoxRenderer(typeof(FriendUser), typeof(FriendListBoxItemRenderer))]