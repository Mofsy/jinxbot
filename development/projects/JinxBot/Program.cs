using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using JinxBot.WebProtocols;
using JinxBot.Views.Chat;

namespace JinxBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NonAdminComRegistration.Register<ImageChatNodeProtocol>();
            ImageChatNodeProtocol.RegisterTemporary();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());

            ImageChatNodeProtocol.Unregister();
            NonAdminComRegistration.Unregister<ImageChatNodeProtocol>();
        }
    }
}
