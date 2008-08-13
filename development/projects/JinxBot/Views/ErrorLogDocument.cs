using JinxBot.Controls.Docking;
using JinxBot.Controls;
using JinxBot.Views.Chat;

namespace JinxBot.Views
{
    public partial class ErrorLogDocument : DockableDocument
    {
        public ErrorLogDocument()
        {
            InitializeComponent();
        }

        public void AddError(string message)
        {
            display.AddChat(new ChatNode(message, CssClasses.Error));
        }
    }
}
