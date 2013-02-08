using BNSharp.BattleNet;
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
        static void Main(string[] args)
        {
            Console.BufferWidth = 300;
            Console.BufferHeight = 4000;

            var settings = new BattleNetSettings();
            var client = new BattleNetClient(settings);

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

        static void Client_Connected()
        {
            PrintTidTs(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("DarkTemplar~AoA on USEast ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("connected.");
        }

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
    }
}
