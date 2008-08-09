using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp;
using BNSharp.Net;

namespace BNSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Logging on with Starcraft; enter CD key.");
            string cdKey = Console.ReadLine().Trim();
            Console.WriteLine("Enter account name:");
            string acct = Console.ReadLine().Trim();
            Console.WriteLine("Enter account password for {0}:", acct);
            string password = Console.ReadLine().Trim();

            Settings set = new Settings() { CdKey1 = cdKey, Username = acct, Password = password, CdKeyOwner = acct };

            BattleNetClient client = new BattleNetClient(set);
            client.Connected += delegate { Console.WriteLine("--- CONNECTED"); };
            client.Error += new ErrorEventHandler(client_Error);
            client.EnteredChat += new EnteredChatEventHandler(client_EnteredChat);
            client.LoginSucceeded += new EventHandler(client_LoginSucceeded);
            client.LoginFailed += new LoginFailedEventHandler(client_LoginFailed);
            client.ServerBroadcast += new ServerChatEventHandler(client_ServerBroadcast);
            client.ServerErrorReceived += new ServerChatEventHandler(client_ServerErrorReceived);
            client.UserShown += new UserEventHandler(client_UserShown);
            client.UserJoined += new UserEventHandler(client_UserJoined);
            client.UserLeft += new UserEventHandler(client_UserLeft);
            client.UserSpoke += new ChatMessageEventHandler(client_UserSpoke);
            client.UserEmoted += new ChatMessageEventHandler(client_UserEmoted);
            client.ClientCheckPassed += delegate { Console.WriteLine("--- VERSIONING PASSED"); };
            client.ClientCheckFailed += new ClientCheckFailedEventHandler(client_ClientCheckFailed);
            client.JoinedChannel += new ServerChatEventHandler(client_JoinedChannel);
            client.WardentUnhandled += delegate { Console.WriteLine("--- WARNING: Warden requested and unhandled!!"); };
            client.MessageSent += new ChatMessageEventHandler(client_MessageSent);
            client.Disconnected += delegate { Console.WriteLine("--- DISCONNECTED"); };

            BattleNetClientResources.IncomingBufferPool.NewBufferAllocated += new EventHandler(BufferAllocated);
            BattleNetClientResources.OutgoingBufferPool.NewBufferAllocated += new EventHandler(BufferAllocated);

            Console.WriteLine("Events hooked up; press <enter> to connect.");
            Console.ReadLine();

            Console.WriteLine("Type /exit to quit.");

            client.Connect();

            string text;
            bool exit = false;
            do
            {
                text = Console.ReadLine();
                if (!text.Equals("/exit", StringComparison.Ordinal))
                {
                    client.SendMessage(text);
                }
                else
                {
                    exit = true;
                }
            } while (!exit);

            client.Close();
            Console.WriteLine("Disconnected; press <enter> to exit.");
            Console.ReadLine();

        }

        static void client_MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (e.EventType == ChatEventType.Emote)
            {
                Console.WriteLine("<{0} {1}>", e.Username, e.Text);
            }
            else
            {
                Console.WriteLine("[{0}]: {1}", e.Username, e.Text);
            }
        }

        static void BufferAllocated(object sender, EventArgs e)
        {
            Console.WriteLine("=+=+= BUFFER ALLOCATED =+=+=");
            Console.WriteLine((sender as BufferPool).Name);
        }

        static void client_JoinedChannel(object sender, ServerChatEventArgs e)
        {
            Console.WriteLine("CHANNEL: {0}", e.Text);
        }

        static void client_ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            Console.WriteLine("--- VERSIONING FAILED {0}:", e.Reason);
            Console.WriteLine(e.AdditionalInformation);
        }

        static void client_UserEmoted(object sender, ChatMessageEventArgs e)
        {
            Console.WriteLine("<{0} {1}>", e.Username, e.Text);
        }

        static void client_UserSpoke(object sender, ChatMessageEventArgs e)
        {
            Console.WriteLine("{0}: {1}", e.Username, e.Text);
        }

        static void client_UserLeft(object sender, UserEventArgs e)
        {
            Console.WriteLine("USER LEFT: {0}", e.User.Username);
        }

        static void client_UserJoined(object sender, UserEventArgs e)
        {
            Console.WriteLine("USER JOIN: {0} ({1})", e.User.Username, e.User.Stats.LiteralText);
        }

        static void client_UserShown(object sender, UserEventArgs e)
        {
            Console.WriteLine("USER: {0} ({1})", e.User.Username, e.User.Stats.LiteralText);
        }

        static void client_ServerErrorReceived(object sender, ServerChatEventArgs e)
        {
            Console.WriteLine("SERVER ERROR: {0}", e.Text);
        }

        static void client_ServerBroadcast(object sender, ServerChatEventArgs e)
        {
            Console.WriteLine("SERVER: {0}", e.Text);
        }

        static void client_LoginFailed(object sender, LoginFailedEventArgs e)
        {
            Console.WriteLine("--- LOGIN FAILED:");
        }

        static void client_LoginSucceeded(object sender, EventArgs e)
        {
            Console.WriteLine("--- LOGIN SUCCEEDED");
        }

        static void client_EnteredChat(object sender, EnteredChatEventArgs e)
        {
            Console.WriteLine("Entered chat as {0}", e.UniqueUsername);
        }

        static void client_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("ERROR: {0}", e.Error);
        }
    }

    class Settings : IBattleNetSettings
    { 
        #region IBattleNetSettings Members

        public string Client
        {
            get
            {
                return "STAR";
            }
            set
            {
                
            }
        }

        public int VersionByte
        {
            get
            {
                return 0xd1;
            }
            set
            {
               
            }
        }

        public string CdKey1
        {
            get;
            set;
        }

        public string CdKey2
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public string GameExe
        {
            get
            {
                return @"C:\Gamefiles\STAR\Starcraft.exe";
            }
            set
            {
                
            }
        }

        public string GameFile2
        {
            get
            {
                return @"C:\Gamefiles\STAR\Storm.dll";
            }
            set
            {
                
            }
        }

        public string GameFile3
        {
            get
            {
                return @"C:\Gamefiles\STAR\Battle.snp";
            }
            set
            {
                
            }
        }

        public string Username
        {
            get;
            set;
        }

        public string ImageFile
        {
            get
            {
                return @"C:\Gamefiles\STAR\STAR.bin";
            }
            set
            {
                
            }
        }

        public string Password
        {
            get;
            set;
        }

        public string Server
        {
            get
            {
                return "useast.battle.net";
            }
            set
            {
                
            }
        }

        public int Port
        {
            get
            {
                return 6112;
            }
            set
            {
                
            }
        }

        public string CdKeyOwner
        {
            get;
            set;
        }

        #endregion

        #region IBattleNetSettings Members


        public BNSharp.BattleNet.PingType PingMethod
        {
            get { return BNSharp.BattleNet.PingType.Normal; }
            set { }
        }

        #endregion
    }
}
