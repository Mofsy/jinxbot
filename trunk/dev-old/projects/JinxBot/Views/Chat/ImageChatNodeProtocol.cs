using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using JinxBot.WebProtocols;
using System.Reflection;

namespace JinxBot.Views.Chat
{
    [Guid("2D7B8E47-2142-433b-B9AB-3E6DA48D7415")]
    internal sealed class ImageChatNodeProtocol : PluggableProtocolHandler
    {
        public const string Schema = "cnimage";

        protected override void OnProtocolStart(EventArgs e)
        {
            base.OnProtocolStart(e);

            if (images.ContainsKey(Request.Url.LocalPath))
            {
                Response.ContentType = "image/jpeg";
                byte[] imageData = images[Request.Url.LocalPath];
                Response.OutputStream.Write(imageData, 0, imageData.Length);
                Response.EndResponse();
            }
            else
            {
                throw new Exception("Specified image has not been queued.");
            }
        }

        protected override void OnProtocolAbort(AbortEventArgs e)
        {
            base.OnProtocolAbort(e);
        }

        #region Registrations

        static readonly object registerSync = new object();
        static bool registered = false;
        static bool permanent = false;

        public static void RegisterTemporary()
        {
            lock (registerSync)
            {
                if (registered)
                    throw new InvalidOperationException("Protocol already registered.");

                PluggableProtocolRegistrationServices.RegisterTemporaryProtocolHandler(
                    MethodBase.GetCurrentMethod().DeclaringType,
                    Schema);

                registered = true;
                permanent = false;
            }
        }

        public static void RegisterPermanent()
        {
            lock (registerSync)
            {
                if (registered)
                    throw new InvalidOperationException("Protocol already registered.");

                PluggableProtocolRegistrationServices.RegisterPermanentProtocolHandler(
                    MethodBase.GetCurrentMethod().DeclaringType,
                    Schema);

                registered = true;
                permanent = true;
            }
        }

        public static void Unregister()
        {
            lock (registerSync)
            {
                if (!registered)
                    throw new InvalidOperationException("Protocol not yet registered.");

                if (permanent)
                    PluggableProtocolRegistrationServices.UnregisterPermanentProtocolHandler(
                        MethodBase.GetCurrentMethod().DeclaringType,
                        Schema);
                else
                    PluggableProtocolRegistrationServices.UnregisterTemporaryProtocolHandler(
                        MethodBase.GetCurrentMethod().DeclaringType,
                        Schema);

                registered = false;
                permanent = false;
            }
        }

        #endregion

        private static Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();

        internal static bool HasRegistered(string imageName)
        {
            return images.ContainsKey(imageName);
        }

        internal static void RegisterImage(string imageName, byte[] memory)
        {
            if (!images.ContainsKey(imageName))
                images.Add(imageName, memory);
        }
    }
}
