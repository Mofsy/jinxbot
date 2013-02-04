using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.BattleNet;
using BNSharp;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using JinxBot.Plugins.Data;

namespace JinxBot.Plugins
{
    internal sealed class CommandTranslator : IDisposable
    {
        private static Regex MetacharacterEscape = new Regex(@"[][{}()*+?.\\^$|]");
        private static string MetacharacterReplace = @"\\$0";

        private JinxBotClient m_client;
        private ClientProfile m_profile;
        private ChatMessageEventHandler userSpoke, whisperReceived;
        private Regex m_speakingTest;

        public CommandTranslator(JinxBotClient client)
        {
            Debug.Assert(client != null);

            m_client = client;
            m_profile = client.Client.Settings as ClientProfile;
            string triggerCharacter = m_profile.TriggerCharacter;
            if (string.IsNullOrEmpty(triggerCharacter))
                triggerCharacter = "!";
            triggerCharacter = MetacharacterEscape.Replace(triggerCharacter, MetacharacterReplace);
            m_speakingTest = new Regex(string.Format(CultureInfo.InvariantCulture, "\\A{0}(?<commandText>.+)", triggerCharacter));

            userSpoke = client_UserSpoke;
            whisperReceived = client_WhisperReceived;

            client.Client.UserSpoke += userSpoke;
            client.Client.WhisperReceived += whisperReceived;
            client.Client.RegisterMessageSentNotification(Priority.Low, userSpoke);
        }

        private void client_UserSpoke(object sender, ChatMessageEventArgs e)
        {
            Match m = m_speakingTest.Match(e.Text);
            if (m.Success)
            {
                string contents = m.Groups["commandText"].Value;
                ProcessCommand(e, contents);
            }
        }

        private void ProcessCommand(ChatMessageEventArgs e, string contents)
        {
            string[] commandParts = contents.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = null;
            string[] parameters = new string[commandParts.Length - 1];
            if (commandParts.Length > 0)
                command = commandParts[0];
            if (commandParts.Length > 1)
            {
                Array.Copy(commandParts, 1, parameters, 0, parameters.Length);
            }
            IJinxBotPrincipal commander = m_client.Database.FindUsers(e.Username).FirstOrDefault();
            if (commander != null)
            {
                foreach (ICommandHandler handler in m_client.CommandHandlers)
                {
                    if (handler.HandleCommand(commander, command, parameters))
                        break;
                }
            }
        }

        private void client_WhisperReceived(object sender, ChatMessageEventArgs e)
        {
            ProcessCommand(e, e.Text);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_client.Client.UnregisterMessageSentNotification(Priority.Low, userSpoke);
                m_client.Client.WhisperReceived -= whisperReceived;
                m_client.Client.UserSpoke -= userSpoke;

                whisperReceived = null;
                userSpoke = null;
                m_client = null;
                m_profile = null;
            }
        }

        #endregion
    }
}
