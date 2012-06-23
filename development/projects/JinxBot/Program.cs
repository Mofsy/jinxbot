using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using JinxBot.WebProtocols;
using JinxBot.Views.Chat;
using JinxBot.Reliability;
using System.Diagnostics;
using JinxBot.Configuration;

namespace JinxBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool newMutexCreated = false;
            string mutexName = "Local\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Mutex mutex = null;
            try
            {
                mutex = new Mutex(false, mutexName, out newMutexCreated);
            }
            catch 
            {
            }

            if (newMutexCreated)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                GlobalErrorHandler.Initialize();
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

                NonAdminComRegistration.Register<ImageChatNodeProtocol>();
                ImageChatNodeProtocol.RegisterTemporary();

                var win = new MainWindow(args);

                Application.Run(win);

                ImageChatNodeProtocol.Unregister();
                NonAdminComRegistration.Unregister<ImageChatNodeProtocol>();
            }
            else
            {
                Process[] currentProcesses = (from p in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
                                              where p.Id != Process.GetCurrentProcess().Id
                                              select p).ToArray();
                if (currentProcesses.Length > 0)
                {
                    IntPtr mainWindowHandle = currentProcesses[0].MainWindowHandle;
                    InstanceManagementClient client = new InstanceManagementClient(currentProcesses[0].Id);
                    if (mainWindowHandle != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.ShowWindow(mainWindowHandle, 9); //SW_RESTORE=9
                        UnsafeNativeMethods.UpdateWindow(mainWindowHandle);
                    }
                    client.InvokeParameter(args);
                }
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            GlobalErrorHandler.ReportException(e.Exception);
        }
    }
}
