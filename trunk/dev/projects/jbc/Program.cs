using BNSharp.BattleNet;
using BNSharp.Chat;
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
        static void Main(string[] args)
        {
            Console.BufferWidth = 300;
            Console.BufferHeight = 4000;

            _settings = new BattleNetSettings();
            var client = new BattleNetClient(_settings);

            client.Connected += Client_Connected;
            client.Disconnected += Client_Disconnected;
            client.ClientCheckPassed += client_ClientCheckPassed;
            client.ClientCheckFailed += client_ClientCheckFailed;
            client.LoginSucceeded += client_LoginSucceeded;
            client.LoginFailed += client_LoginFailed;
            client.AccountCreated += client_AccountCreated;
            client.AccountCreationFailed += client_AccountCreationFailed;


            client.ConnectAsync();

            string lastInput;
            do
            {
                lastInput = Console.ReadLine();
                if (lastInput != "/quit")
                {

                }
            }
            while (lastInput != "/quit");
        }

        static void client_AccountCreationFailed(object sender, AccountCreationFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to create account ({0}).", e.Reason);
            LoginWithNewCredentials(sender);
        }

        static void client_AccountCreated(object sender, AccountCreationEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Account successfully created.");
        }

        static void PrintTid()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[{0}] ", Thread.CurrentThread.ManagedThreadId);
        }

        static void PrintTidTs(DateTime when)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[{0}] [{1:hh.mm.ss}] ", Thread.CurrentThread.ManagedThreadId, when);
        }

        static void Client_Connected(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("{0} on {1} ", _settings.Username, _settings.Gateway.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("connected.");
        }

        static void Client_Disconnected(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Disconnected.");
        }

        static void client_LoginFailed(object sender, BNSharp.Chat.LoginFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Login failed ({0}) for ", e.Reason);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(_settings.Username);

            if (e.Reason == LoginFailureReason.AccountDoesNotExist)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Attempting to create a new account using the provided credentials...");
                (sender as BattleNetClient).CreateAccount(_settings.Username, _settings.Password);
            }
            else
            {
                LoginWithNewCredentials(sender);
            }
        }

        private static void LoginWithNewCredentials(object sender)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Please enter new credentials.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Username: ");
            _settings.Username = Console.ReadLine();
            Console.Write("Password: ");
            _settings.Password = Console.ReadLine();

            BattleNetClient client = sender as BattleNetClient;
            client.ContinueLogin();
        }

        static void client_LoginSucceeded(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Login succeeded.");
        }

        static void client_ClientCheckFailed(object sender, ClientCheckFailedEventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Version check failed, disconnecting...");
            Console.WriteLine("Error note: {0}", e.Reason);
            (sender as BattleNetClient).Disconnect();
        }

        static void client_ClientCheckPassed(object sender, EventArgs e)
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Version check passed.  Attempting to log in...");
        }


        #region unused so far
        static void Client_JoinedChannel()
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Joining channel ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The Nest");
        }

        static void Client_UserJoinedChannel()
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("sno.man ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("joined the channel using ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Starcraft: Brood War");
        }

        static void Client_UserLeftChannel()
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("sno.man");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" left the channel.");
        }

        static void Client_UserWasInChannel()
        {

        }

        static void Client_UserSpoke()
        {

        }

        static void Client_UserEmoted()
        {

        }

        static void Client_WhisperReceived()
        {

        }

        static void Client_WhisperSent()
        {

        }

        static void Client_MessageSent()
        {

        }

        #endregion
    }
}
