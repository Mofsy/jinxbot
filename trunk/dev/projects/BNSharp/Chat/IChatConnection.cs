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
        Task<bool> ConnectAsync();
        void Send(string text);
        void CreateAccount(string accountName, string password);
        void ContinueLogin();
        bool IsConnected { get; }

        event EventHandler LoginSucceeded;
        event EventHandler<LoginFailedEventArgs> LoginFailed;
        event EventHandler<AccountCreationEventArgs> AccountCreated;
        event EventHandler<AccountCreationFailedEventArgs> AccountCreationFailed;
        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler<string> MessageSent;

        event EventHandler<ServerChatEventArgs> Broadcast;
        event EventHandler<ServerChatEventArgs> ServerError;
        event EventHandler<ServerChatEventArgs> ServerInformation;
    }

    public interface IChatConnectionEventSource
    {
        void OnLoginSucceeded();
        void OnLoginFailed(LoginFailedEventArgs args);
        void OnAccountCreated(AccountCreationEventArgs args);
        void OnAccountCreationFailed(AccountCreationFailedEventArgs args);
        void OnConnected();
        void OnDisconnected();
        void OnMessageSent(string message);

        void OnBroadcast(ServerChatEventArgs args);
        void OnServerError(ServerChatEventArgs args);
        void OnServerInformation(ServerChatEventArgs args);
    }
#pragma warning restore 1591
}
