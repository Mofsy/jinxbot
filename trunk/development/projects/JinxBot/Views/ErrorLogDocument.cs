using JinxBot.Controls.Docking;
using JinxBot.Controls;
using JinxBot.Views.Chat;
using JinxBot.Plugins.UI;

namespace JinxBot.Views
{
    public partial class ErrorLogDocument : DockableDocument, IChatTab
    {
        public ErrorLogDocument()
        {
            InitializeComponent();
        }

        public void AddError(string message)
        {
            display.AddChat(new ChatNode(message, CssClasses.Error));
        }

        #region IChatTab Members

        public System.Uri StylesheetUri
        {
            get;
            set;
        }

        #endregion
    }
}
