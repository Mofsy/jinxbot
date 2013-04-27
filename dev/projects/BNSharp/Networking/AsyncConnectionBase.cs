using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.Networking
{
    /// <summary>
    /// Represents a asynchronous TCP/IP connection.
    /// </summary>
    public class AsyncConnectionBase : IDisposable
    {
        private TcpClient _client;
        private IPEndPoint _ipep;
        private string _server;
        private int _port;
        private bool _open;

        private NetworkBufferStorage _storage;

        /// <summary>
        /// Creates a new instance of the AsyncConnectionBase class.
        /// </summary>
        /// <param name="server">The URI of the server to connect to.</param>
        /// <param name="port">The port of the server to connect to.</param>
        public AsyncConnectionBase(string server, int port)
            : this(server, port, 8192, 50)
        {
            
        }

        internal AsyncConnectionBase(string server, int port, int perPacketSize, int bufferSize)
        {
            _server = server;
            _port = port;

            _storage = new NetworkBufferStorage(perPacketSize, bufferSize);
        }

        #region IDisposable Members

        ~AsyncConnectionBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the specified object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object, freeing unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">Whether to free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null && _client.Connected)
                {
                    Close();
                }
                _client = null;
                _ipep = null;
                _open = false;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets whether the connection is alive or not.
        /// </summary>
        public bool IsConnected { get { return _open; } }

        /// <summary>
        /// Once the connection is established, gets the local endpoint to which the client is bound.
        /// </summary>
        public IPEndPoint LocalEP
        {
            get
            {
                return (IPEndPoint)_client.Client.LocalEndPoint;
            }
        }

        /// <summary>
        /// Once the connection is established, gets the remote endpoint to which the client is bound.
        /// </summary>
        public IPEndPoint RemoteEP
        {
            get
            {
                return (IPEndPoint)_client.Client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// Allows derived classes to always require the connection to re-resolve the remote host during the <see cref="ConnectAsync()"/> method.
        /// </summary>
        protected virtual bool AlwaysResolveRemoteHost
        {
            get { return false; }
        }

        internal NetworkBufferStorage NetworkBuffers
        {
            get { return _storage; }
        }
        #endregion

        #region ConnectionErrorOccurred event
        /// <summary>
        /// When overridden by a derived class, provides error information from the current connection.
        /// </summary>
        /// <param name="message">Human-readable information about the error.</param>
        /// <param name="ex">An internal exception containing the error details.</param>
        protected virtual void OnConnectionErrorOccurred(string message, Exception ex)
        {
            if (ConnectionErrorOccurred != null)
                ConnectionErrorOccurred(this, new ConnectionErrorEventArgs(message, ex));
        }

        /// <summary>
        /// Represents an error that occurred.
        /// </summary>
        public event EventHandler<ConnectionErrorEventArgs> ConnectionErrorOccurred;
        #endregion

        /// <summary>
        /// Asynchronously resolves an IP end point for the specified server and port.
        /// </summary>
        /// <param name="server">The DNS name or IP address (as a string) of the server to look up.</param>
        /// <param name="port">The port number to which to connect.</param>
        /// <returns>An <see>IPEndPoint</see> representing the server and port; or, if resolution failed, <see langword="null" />.</returns>
        protected virtual async Task<IPEndPoint> ResolveEndpointAsync(string server, int port)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(server);
            foreach (IPAddress addr in addresses)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    return new IPEndPoint(addr, port);
            }
            return null;
        }

        /// <summary>
        /// Connects the connection to a remote host other than the one for which the connection was initialized.
        /// </summary>
        /// <param name="server">The URI of the server to which to connect.</param>
        /// <param name="port">The port of the server to which to connect.</param>
        /// <returns><see langword="true" /> if the connection completed successfully; otherwise <see langword="false" />.</returns>
        /// <remarks>
        /// <para>The <see cref="OnConnectionErrorOccurred">OnConnectionErrorOccurred</see> method is called when an error takes place, which
        /// fires the <see cref="ConnectionErrorOccurred"/> event.</para>
        /// <para>This method may return false if the connection is already established.  To check whether the 
        /// connection is established, check the <see>IsConnected</see> property.</para>
        /// </remarks>
        public Task<bool> ConnectAsync(string server, int port)
        {
            _server = server;
            _port = port;

            return ConnectAsync();
        }

        /// <summary>
        /// Connects the connection to the remote host.
        /// </summary>
        /// <returns><b>True</b> if the connection completed successfully; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>The <see cref="OnConnectionErrorOccurred">OnConnectionErrorOccurred</see> method is called when an error 
        /// takes place, which fires the <see cref="ConnectionErrorOccurred"/> event.</para>
        /// <para>This method may return false if the connection is already established.  To check whether the 
        /// connection is established, check the <see>IsConnected</see> property.</para>
        /// </remarks>
        public virtual async Task<bool> ConnectAsync()
        {
            // sanity check
            if (_open)
                return false;

            if (_ipep == null || AlwaysResolveRemoteHost)
            {
                try
                {
                    _ipep = await ResolveEndpointAsync(_server, _port);
                    if (_ipep == null)
                        //throw new SocketException("Could not resolve the endpoint based on target server and port.");
                        throw new SocketException();
                }
                catch (SocketException se)
                {
                    OnConnectionErrorOccurred(
                        string.Format("Your computer was unable to resolve hostname {0}.  If necessary, add an entry to %SystemRoot%\\system32\\drivers\\etc\\hosts, or flush your DNS resolver cache, and try again.", _server), 
                        se);
                    return false;
                }
            }
            try
            {
                _open = true;
                _client = new TcpClient();
                _client.NoDelay = true;
                await _client.ConnectAsync(_ipep.Address, _ipep.Port);
            }
            catch (SocketException se)
            {
                _open = false;
                OnConnectionErrorOccurred(
                    string.Format("The connection was unable to complete a connection to {0}:{1} ({2}).  More information is available in the exception.", _server, _port, _ipep.Address.ToString()), se);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Closes the connection and prepares for a new connection.
        /// </summary>
        public virtual void Close()
        {
            if (_open)
            {
                _open = false;
                _client.Client.Disconnect(true);
            }
        }

        internal async Task<byte[]> ReceiveAsync(byte[] target, int startIndex, int count)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Index must refer to a location within the array.");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, "Amount of data to receive must be a nonnegative integer.");
            if (startIndex + count > target.Length)
                throw new ArgumentOutOfRangeException("count", count, "Cannot receive past the end of the destination array.");

            int totRecv = 0;
            var stream = _client.GetStream();
            while (_open && _client.Connected && totRecv < count)
            {
                try
                {
                    totRecv += await stream.ReadAsync(target, totRecv + startIndex, count - totRecv);
                }
                catch (IOException se)
                {
                    if (IsConnected)
                    {
                        Close();
                        OnConnectionErrorOccurred("A read error occurred on the connection.", se);
                    }

                    return null;
                }
            }

            return target;
        }

        /// <summary>
        /// Asynchronously receives an amount of data.
        /// </summary>
        /// <param name="length">The amount of data to receive.</param>
        /// <returns>A NetworkBuffer containing the data.</returns>
        public async Task<NetworkBuffer> ReceiveAsync(int length)
        {
            NetworkBuffer buffer = _storage.Acquire();
            if (buffer == null)
                throw new InvalidOperationException("No network storage could be retrieved.");

            if (length > buffer.Length)
                throw new ArgumentOutOfRangeException("length", length, "The underlying storage for network data was not large enough for the requested receive operation.");

            return await ReceiveAsync(buffer, 0, length);
        }

        /// <summary>
        /// Asynchronously receives an amount of data into a preexisting network buffer.
        /// </summary>
        /// <param name="partialDestination"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<NetworkBuffer> ReceiveAsync(NetworkBuffer partialDestination, int startIndex, int length, bool assertHeader = false)
        {
            if (partialDestination == null)
                throw new ArgumentNullException("partialDestination");

            if (length < 1)
                throw new ArgumentOutOfRangeException("length", length, "Length must be a positive integer.");

            if (startIndex >= partialDestination.UnderlyingBuffer.Length)
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Start index is outside of the range of the destination buffer.");
            if (startIndex + length >= partialDestination.UnderlyingBuffer.Length)
                throw new ArgumentException("Length given start index end outside of the bounds of the destination buffer.");

            int totRecv = 0;
            var stream = _client.GetStream();
            while (_open && _client.Connected && totRecv < length)
            {
                try
                {
                    totRecv += await stream.ReadAsync(partialDestination.UnderlyingBuffer, partialDestination.StartingPosition + totRecv + startIndex, length - totRecv);
                }
                catch (IOException se)
                {
                    if (IsConnected)
                    {
                        Close();
                        OnConnectionErrorOccurred("A read error occurred on the connection.", se);
                    }

                    return null; 
                }
            }

            if (assertHeader)
                Debug.Assert(partialDestination.UnderlyingBuffer[startIndex + partialDestination.StartingPosition] == 0xff);

            return partialDestination;
        }

        /// <summary>
        /// Sends the specified binary data to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public virtual async Task SendAsync(byte[] data)
        {
            await SendAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Sends part of the specified binary data to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="index">The start index of the data.</param>
        /// <param name="length">The amount of data to send.</param>
        public virtual async Task SendAsync(byte[] data, int index, int length)
        {
            await _client.GetStream().WriteAsync(data, index, length);
        }

        /// <summary>
        /// Sends the specified binary data to the server.  This method automatically releases the network buffer upon completion.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public virtual async Task SendAsync(NetworkBuffer data, int length)
        {
            await SendAsync(data, 0, length);
        }

        /// <summary>
        /// Sends part of the specified binary data to the server.  This method automatically releases the network buffer upon completion.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="index">The start index of the data.</param>
        /// <param name="length">The amount of data to send.</param>
        public virtual async Task SendAsync(NetworkBuffer data, int index, int length)
        {
            await SendAsync(data.UnderlyingBuffer, data.StartingPosition + index, length);
            _storage.Release(data);
        }
    }
}
