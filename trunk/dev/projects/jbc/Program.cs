using BNSharp.BattleNet;
using BNSharp.Chat;
using Paveza;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jbc
{
    class Program
    {
        private static BattleNetSettings _settings;
        private static AsyncConsole console;

        private static EventWaitHandle _ended;
        static void Main(string[] args)
        {
            _settings = new BattleNetSettings();

            _ended = new EventWaitHandle(false, EventResetMode.ManualReset);
            Thread t = new Thread(MainThread);
            t.IsBackground = false;
            t.Priority = ThreadPriority.Normal;
            t.Start();

            _ended.WaitOne();
        }

        private static async void MainThread()
        {
            console = new AsyncConsole(200, 300);
            console.InputForegroundColor = ConsoleColor.White;
            console.OutputBackgroundColor = ConsoleColor.Black;
            console.OutputForegroundColor = ConsoleColor.White;
            console.InputBackgroundColor = ConsoleColor.Black;
            console.WindowWidth = 200;
            console.Clear();

            var client = new BattleNetClient(_settings);

            client.Connected += Client_Connected;
            client.Disconnected += Client_Disconnected;
            client.ClientCheckPassed += client_ClientCheckPassed;
            client.ClientCheckFailed += client_ClientCheckFailed;
            client.LoginSucceeded += client_LoginSucceeded;
            client.LoginFailed += client_LoginFailed;
            client.AccountCreated += client_AccountCreated;
            client.AccountCreationFailed += client_AccountCreationFailed;

            client.Channel.UserJoined += Client_UserJoinedChannel;
            client.Channel.UserLeft += Client_UserLeftChannel;
            client.Channel.UserShown += Client_UserWasInChannel;
            client.Channel.UserSpoke += Client_UserSpoke;
            client.Channel.UserEmoted += Client_UserEmoted;
            client.Channel.NewChannelJoined += Channel_NewChannelJoined;

            client.ServerError += client_ServerError;
            client.ServerInformation += client_ServerInformation;
            client.Broadcast += client_Broadcast;
            client.WhisperReceived += client_WhisperReceived;
            client.WhisperSent += client_WhisperSent;
            client.ChannelListReceived += client_ChannelListReceived;

            client.ConnectAsync();

            string lastInput;
            do
            {
                lastInput = await console.ReadLineAsync();
                switch (lastInput)
                {
                    case "/clear":
                        console.Clear();
                        break;
                    case "/channel-list":
                    case "/cl":
                        if (_channel != null)
                        {
                            client_ChannelListReceived(client, _channel);
                        }
                        else
                        {
                            console.OutputForegroundColor = ConsoleColor.Red;
                            console.WriteLine("The channel list has not yet been received.");
                        }
                        break;
                    case "/quit":
                        client.Disconnect();
                        break;
                    default:
                        client.Send(lastInput);
                        break;
                }
            }
            while (lastInput != "/quit");

            _ended.Set();
        }

        private static ChannelListEventArgs _channel;

        static void client_ChannelListReceived(object sender, ChannelListEventArgs e)
        {
            _channel = e;
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.WriteLine("Available channels:");
            console.OutputForegroundColor = ConsoleColor.Cyan;
            foreach (string name in e.Channels)
            {
                console.WriteLine(" - {0}", name);
            }
        }

        static void Channel_NewChannelJoined(object sender, ServerChatEventArgs e)
        {
            Client_JoinedChannel(sender, e);
        }

        static void client_WhisperSent(object sender, ChatMessageEventArgs<UserFlags> e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Magenta;
            console.WriteLine("You whisper to {0}: {1}", e.Username, e.Text);
        }

        static void client_WhisperReceived(object sender, ChatMessageEventArgs<UserFlags> e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Magenta;
            console.WriteLine("{0} whispers: {1}", e.Username, e.Text);
        }

        static void client_Broadcast(object sender, ServerChatEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.WriteLine("Broadcast: {0}", e.Text);
        }

        static void client_ServerInformation(object sender, ServerChatEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Cyan;
            console.WriteLine(e.Text);
        }

        static void client_ServerError(object sender, ServerChatEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Red;
            console.WriteLine(e.Text);
        }

        static void client_AccountCreationFailed(object sender, AccountCreationFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Red;
            console.WriteLine("Failed to create account ({0}).", e.Reason);
            LoginWithNewCredentials(sender);
        }

        static void client_AccountCreated(object sender, AccountCreationEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Cyan;
            console.WriteLine("Account successfully created.");
        }

        static void PrintTid()
        {
            console.OutputForegroundColor = ConsoleColor.Green;
            console.Write("[{0}] ", Thread.CurrentThread.ManagedThreadId);
        }

        static void PrintTidTs(DateTime when)
        {
            console.OutputForegroundColor = ConsoleColor.Green;
            console.Write("[{0}] [{1:hh.mm.ss}] ", Thread.CurrentThread.ManagedThreadId, when);
        }

        static void Client_Connected(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.Write("{0} on {1} ", _settings.Username, _settings.Gateway.Name);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.WriteLine("connected.");
        }

        static void Client_Disconnected(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Red;
            console.WriteLine("Disconnected.");
        }

        static void client_LoginFailed(object sender, BNSharp.Chat.LoginFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Red;
            console.Write("Login failed ({0}) for ", e.Reason);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.WriteLine(_settings.Username);

            if (e.Reason == LoginFailureReason.AccountDoesNotExist)
            {
                console.OutputForegroundColor = ConsoleColor.Blue;
                console.WriteLine("Attempting to create a new account using the provided credentials...");
                (sender as BattleNetClient).CreateAccount(_settings.Username, _settings.Password);
            }
            else
            {
                LoginWithNewCredentials(sender);
            }
        }

        private static void LoginWithNewCredentials(object sender)
        {
            console.OutputForegroundColor = ConsoleColor.Cyan;
            console.WriteLine("Please enter new credentials.");
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.Write("Username: ");
            _settings.Username = Console.ReadLine();
            console.Write("Password: ");
            _settings.Password = Console.ReadLine();

            BattleNetClient client = sender as BattleNetClient;
            client.ContinueLogin();
        }

        static void client_LoginSucceeded(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);

            console.OutputForegroundColor = ConsoleColor.Green;
            console.WriteLine("Login succeeded.");
        }

        static void client_ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.DarkMagenta;
            console.WriteLine("Version check failed, disconnecting...");
            console.WriteLine("Error note: {0}", e.Reason);
            (sender as BattleNetClient).Disconnect();
        }

        static void client_ClientCheckPassed(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Green;
            console.WriteLine("Version check passed.  Attempting to log in...");
        }


        #region unused so far
        static void Client_JoinedChannel(object sender, ServerChatEventArgs args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.Write("Joining channel ");
            console.OutputForegroundColor = ConsoleColor.White;
            console.WriteLine(args.Text);
        }

        static void Client_UserJoinedChannel(object sender, UserEventArgs<ChatUser> args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.Write(args.User.Username);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.Write(" joined the channel using ");
            console.OutputForegroundColor = ColorFromProduct(args.User.Stats.Product);
            console.WriteLine(args.User.Stats.Product.Name);
        }

        private static Dictionary<ClassicProduct, ConsoleColor> _colors = new Dictionary<ClassicProduct, ConsoleColor>
        {
            { ClassicProduct.ChatClient, ConsoleColor.Gray },
            { ClassicProduct.Diablo2Expansion, ConsoleColor.DarkRed },
            { ClassicProduct.Diablo2Retail, ConsoleColor.DarkBlue },
            { ClassicProduct.Diablo2Shareware, ConsoleColor.DarkBlue },
            { ClassicProduct.DiabloRetail, ConsoleColor.DarkYellow },
            { ClassicProduct.DiabloShareware, ConsoleColor.DarkYellow },
            { ClassicProduct.JapanStarcraft, ConsoleColor.Blue },
            { ClassicProduct.StarcraftBroodWar, ConsoleColor.Blue },
            { ClassicProduct.StarcraftRetail, ConsoleColor.Red },
            { ClassicProduct.StarcraftShareware, ConsoleColor.Red },
            { ClassicProduct.UnknownProduct, ConsoleColor.DarkCyan },
            { ClassicProduct.Warcraft2BNE, ConsoleColor.Green },
            { ClassicProduct.Warcraft3Expansion, ConsoleColor.Cyan },
            { ClassicProduct.Warcraft3Retail, ConsoleColor.DarkGreen },
        };

        private static ConsoleColor ColorFromProduct(ClassicProduct product)
        {
            ConsoleColor result;
            if (!_colors.TryGetValue(product, out result))
                result = ConsoleColor.DarkCyan;

            return result;
        }

        static void Client_UserLeftChannel(object sender, UserEventArgs<ChatUser> args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.Write(args.User.Username);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.WriteLine(" left the channel.");


            console.OutputForegroundColor = ConsoleColor.Gray;
        }

        static void Client_UserWasInChannel(object sender, UserEventArgs<ChatUser> args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.Write(args.User.Username);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.Write(" was in the channel using ");
            console.OutputForegroundColor = ColorFromProduct(args.User.Stats.Product);
            console.WriteLine(args.User.Stats.Product.Name);


            console.OutputForegroundColor = ConsoleColor.Gray;
        }

        static void Client_UserSpoke(object sender, ChatMessageEventArgs<UserFlags> args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.Write(args.Username);
            console.OutputForegroundColor = ConsoleColor.Gray;
            console.Write(": ");
            console.OutputForegroundColor = ConsoleColor.White;
            console.WriteLine(args.Text);


            console.OutputForegroundColor = ConsoleColor.Gray;
        }

        static void Client_UserEmoted(object sender, ChatMessageEventArgs<UserFlags> args)
        {
            PrintTidTs(DateTime.Now);
            console.OutputForegroundColor = ConsoleColor.Yellow;
            console.WriteLine("<{0} {1}>", args.Username, args.Text);


            console.OutputForegroundColor = ConsoleColor.Gray;
        }

        #endregion
    }
}
