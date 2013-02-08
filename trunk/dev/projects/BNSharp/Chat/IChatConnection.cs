using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Chat
{
#pragma warning disable 1591
    // TODO: Document these interfaces

    public interface IChatConnection
    {
        Task ConnectAsync();
        void Send(string text);
        void CreateAccount(string accountName, string password);
        void ContinueLogin();
        bool IsConnected { get; }

        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler<string> MessageSent;
    }

    public interface IChatConnectionEventSource
    {
        void OnConnected();
        void OnDisconnected();
        void OnMessageSent(string message);
    }
#pragma warning restore 1591
}
