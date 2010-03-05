using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.ChatLog
{
    public class ChatLogPlugin : ISingleClientPlugin, ICommandHandler
    {
        private ChatLogDocument m_doc;
        private ChatReplayControls m_replay;
        private static string[] HELP = new string[]  {
            "logusers <mm/dd/yyyy> - Lists the users who were observed in the channel during the specified day.",
            "logfind <user> [number-of-messages] - Lists up to number-of-messages of the most recent messages said by the user.", 
            "logseen <user> <days> - Indicates whether the user has been seen in the number of days and, if so, with which client most recently."
        };

        public ChatLogPlugin()
        {
            m_doc = new ChatLogDocument();
            m_replay = new ChatReplayControls();
        }

        #region ISingleClientPlugin Members

        public void CreatePluginWindows(JinxBot.Plugins.UI.IProfileDocument profileDocument)
        {
            profileDocument.AddDocument(m_doc);
            profileDocument.AddToolWindow(m_replay);
        }

        public void DestroyPluginWindows(JinxBot.Plugins.UI.IProfileDocument profileDocument)
        {
            profileDocument.RemoveDocument(m_doc);
            profileDocument.RemoveToolWindow(m_replay);
        }

        #endregion

        #region IJinxBotPlugin Members

        public void Startup(IDictionary<string, string> settings)
        {
            EnumerateLoggedChats();
        }

        private void EnumerateLoggedChats()
        {
            
        }

        public void Shutdown(IDictionary<string, string> settings)
        {
            
        }

        public bool CheckForUpdates()
        {
            return false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_doc != null)
                {
                    m_doc.Dispose();
                    m_doc = null;
                } 
            }
        }

        #region ICommandHandler Members

        public bool HandleCommand(global::BNSharp.BattleNet.ChatUser commander, string command, string[] parameters)
        {
            return false;
        }

        public IEnumerable<string> CommandHelp
        {
            get
            {
                foreach (string item in HELP)
                    yield return item;
            }
        }

        #endregion
    }
}
