using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;
using BNSharp;
using BNSharp.BattleNet.Stats;
using System.Diagnostics;
using System.Timers;

namespace JinxBot.Util
{
    internal sealed class SimulatedBattleNetClient : BattleNetClient
    {
        private Queue<InvokeHelperBase> eventsToFire = new Queue<InvokeHelperBase>();
        private Timer tmrGo;

        #region Invokehelper
        private delegate void Invokee<T>(T args) where T : EventArgs;
        private class InvokeHelperBase
        {
            public virtual void Invoke() { }
        }
        private class InvokeHelper<T> : InvokeHelperBase where T : EventArgs
        {
            public Invokee<T> Target;
            public T Arguments;

            public override void Invoke()
            {
                Target(Arguments);
            }
        }
        #endregion

        public SimulatedBattleNetClient(IBattleNetSettings settings)
            : base(settings)
        {
            tmrGo = new Timer();
            tmrGo.Elapsed += new ElapsedEventHandler(tmrGo_Elapsed);
            tmrGo.AutoReset = true;
            tmrGo.Interval = 75.0;
        }

        void tmrGo_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (eventsToFire.Count > 0)
            {
                InvokeHelperBase item = eventsToFire.Dequeue();
                item.Invoke();
            }
        }

        public override bool Connect()
        {
            eventsToFire.Enqueue(new InvokeHelper<EventArgs> { Target = OnConnected, Arguments = EventArgs.Empty });
            eventsToFire.Enqueue(new InvokeHelper<BaseEventArgs> { Target = OnClientCheckPassed, Arguments = BaseEventArgs.GetEmpty(null) });
            eventsToFire.Enqueue(new InvokeHelper<InformationEventArgs> { Target = OnInformation, Arguments = new InformationEventArgs("Login succeeded!") });
            eventsToFire.Enqueue(new InvokeHelper<BaseEventArgs> { Target = OnLoginSucceeded, Arguments = BaseEventArgs.GetEmpty(null) });
            eventsToFire.Enqueue(new InvokeHelper<EnteredChatEventArgs> { Target = OnEnteredChat, Arguments = new EnteredChatEventArgs(Settings.Username, CreateStatstringForClient(Settings.Client), Settings.Username) });
            eventsToFire.Enqueue(new InvokeHelper<ServerChatEventArgs> { Target = OnServerBroadcast, Arguments = new ServerChatEventArgs(ChatEventType.Information, 0, "Welcome to Battle.net!  This server is simulated.") });
            eventsToFire.Enqueue(new InvokeHelper<ChannelListEventArgs> { Target = OnChannelListReceived, Arguments = new ChannelListEventArgs(new string[] { "Starcraft USA-1", "Clan Recruitment", "The Void", "Public Tech Support", "Blizzard Tech Support" }) });
            EnqueueNewChannelEvents("Starcraft USA-1");
            tmrGo.Start();

            return true;
        }

        public override void ConnectAsync(object state, BNSharp.Net.ConnectCompletedCallback callback)
        {
            Connect();
            callback(new BNSharp.Net.ConnectCompletedResult(state, true));
        }

        private static Product[] productList = new Product[] { Product.StarcraftRetail, Product.StarcraftBroodWar, Product.Warcraft2BNE, Product.Diablo2Retail, Product.Diablo2Expansion, Product.Warcraft3Retail, Product.Warcraft3Expansion };
        private void EnqueueNewChannelEvents(string channelName)
        {
            ChannelName = channelName;
            eventsToFire.Enqueue(new InvokeHelper<ServerChatEventArgs> { Target = OnJoinedChannel, Arguments = new ServerChatEventArgs(ChatEventType.NewChannelJoined, (int)(ChannelFlags.PublicChannel | ChannelFlags.SystemChannel), channelName) });
            Random r = new Random();
            int numUsers = r.Next(0, 20);
            for (int i = 0; i < numUsers; i++)
            {
                string userName = "User_" + i;
                eventsToFire.Enqueue(new InvokeHelper<UserEventArgs> { Target = OnUserShown, Arguments = new UserEventArgs(ChatEventType.UserInChannel, new ChatUser(userName, r.Next(900), UserFlags.None, UserStats.Parse(userName, Encoding.ASCII.GetBytes(CreateStatstringForClient(productList[r.Next(productList.Length)].ProductCode))))) });
            }
            int myPing = r.Next(900);
            if (Settings.PingMethod == PingType.ZeroMs)
                myPing = 0;
            else if (Settings.PingMethod == PingType.MinusOneMs)
                myPing = -1;

            eventsToFire.Enqueue(new InvokeHelper<UserEventArgs> { Target = OnUserJoined, Arguments = new UserEventArgs(ChatEventType.UserJoinedChannel, new ChatUser(Settings.Username, myPing, UserFlags.None, UserStats.Parse(Settings.Username, Encoding.ASCII.GetBytes(CreateStatstringForClient(Settings.Client))))) });

        }

        private string CreateStatstringForClient(string client)
        {
            switch (client.ToUpper())
            {
                case "STAR":
                    return "RATS 0 0 200 0 0 0 0 0 RATS";
                case "SEXP":
                    return "PXES 0 0 200 0 0 0 0 0 PXES";
                case "W2BN":
                    //RATS 0 0 200 0 0 0 0 0 RATS
                    return "NB2W 0 0 200 0 0 0 0 0 NB2W";
                case "D2DV":
                    return "VD2D";
                case "D2XP":
                    return "PX2D";
                case "WAR3":
                    return "3RAW";
                case "W3XP":
                    return "PX3W";
            }

            return "RATS 0 0 200 0 0 0 0 0 RATS";
        }

        public override void ClickAd(int adID)
        {
            
        }

        public override void Close()
        {
            OnDisconnected(EventArgs.Empty);
            tmrGo.Stop();
        }

        public override void ContinueLogin()
        {
            
        }

        public override void DisplayAd(int adID)
        {
            
        }

        public override void JoinChannel(string channelName, JoinMethod method)
        {
            EnqueueNewChannelEvents(channelName);
        }

        public override byte[] Receive(byte[] buffer, int index, int length)
        {
            if (Debugger.IsAttached)
                Debugger.Break();

            return null;
        }

        protected override void Send(byte[] data, int index, int length)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public override void SendMessage(string text)
        {
            if (text.StartsWith("/"))
                eventsToFire.Enqueue(new InvokeHelper<InformationEventArgs> { Target = OnCommandSent, Arguments = new InformationEventArgs(text) });
            else
                eventsToFire.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Target = OnMessageSent, Arguments = new ChatMessageEventArgs(ChatEventType.Talk, UserFlags.None, Settings.Username, text) });
        }

        public override void RequestUserProfile(string accountName, UserProfileRequest profile)
        {
            eventsToFire.Enqueue(new InvokeHelper<UserProfileEventArgs> { Target = OnUserProfileReceived, Arguments = new UserProfileEventArgs(profile) });
        }

        public override void RequestWarcraft3Profile(ChatUser user)
        {
            //eventsToFire.Enqueue(new InvokeHelper<WarcraftProfileEventArgs> { Target = OnWarcraftProfileReceived, Arguments = new WarcraftProfileEventArgs
        }
    }
}
