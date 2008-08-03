using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using JinxBot.WebProtocols;
using JinxBot.Views.Chat;
using JinxBot.Reliability;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalErrorHandler.Initialize();
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            NonAdminComRegistration.Register<ImageChatNodeProtocol>();
            ImageChatNodeProtocol.RegisterTemporary();

            Application.Run(new MainWindow());

            ImageChatNodeProtocol.Unregister();
            NonAdminComRegistration.Unregister<ImageChatNodeProtocol>();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            GlobalErrorHandler.ReportException(e.Exception);
        }
    }
}
