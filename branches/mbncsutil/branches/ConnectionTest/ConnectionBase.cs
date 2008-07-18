/*
MBNCSUtil -- Managed Battle.net Authentication Library
Copyright (C) 2005-2008 by Robert Paveza

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met: 

1.) Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. 
2.) Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution. 
3.) The name of the author may not be used to endorse or promote products derived 
from this software without specific prior written permission. 
	
See LICENSE.TXT that should have accompanied this software for full terms and 
conditions.

*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using MBNCSUtil;
using System.IO;

namespace ConnectionTest
{
    /// <summary>
    /// Represents a TCP/IP connection.
    /// </summary>
    public class ConnectionBase : IDisposable
    {
        private TcpClient m_client;
        private IPEndPoint m_ipep;
        private string m_server;
        private int m_port;

        /// <summary>
        /// Creates a new instance of the <b>ConnectionBase</b> class.
        /// </summary>
        /// <param name="server">The URI of the server to connect to.</param>
        /// <param name="port">The port of the server to connect to.</param>
        public ConnectionBase(string server, int port)
        {
            m_server = server;
            m_port = port;
            try
            {
                m_ipep = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], port);
            }
            catch (SocketException) { }
        }

        /// <summary>
        /// Connects the connection to the remote host.
        /// </summary>
        /// <returns><b>True</b> if the connection completed successfully; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>The <see cref="OnError">OnError</see> method is called when an error takes place, but no
        /// behavior is defined by default.  Inheriting classes should provide an implementation 
        /// to handle errors.</para>
        /// <para>In DEBUG-enabled builds, this class may fail an assertion if <b>Connect</b> is called
        /// while a live socket it still connected.  Always check to ensure that the <see cref="Close">Close</see>
        /// method has been appropriately called first.</para>
        /// </remarks>
        public virtual bool Connect()
        {
            // sanity check
#if DEBUG
            System.Diagnostics.Debug.Assert(!m_open);
#endif

            if (m_ipep == null)
            {
                try
                {
                    m_ipep = new IPEndPoint(Dns.GetHostEntry(m_server).AddressList[0], m_port);
                }
                catch (SocketException se)
                {
                    OnError(string.Format("Your computer was unable to resolve hostname {0}.  If necessary, add an entry to %SystemRoot%\\system32\\drivers\\etc\\hosts, or flush your DNS resolver cache, and try again.",
                        m_server), se);
                    return false;
                }
            }
            try
            {
                m_open = true;
                m_client = new TcpClient();
                m_client.Connect(m_ipep);
            }
            catch (SocketException se)
            {
                m_open = false;
                OnError(string.Format(
                    "The connection was unable to complete a connection to {0}:{1} ({2}).  More information is available in the exception.",
                    m_server, m_port, m_ipep.Address.ToString()), se);
                return false;
            }
            return true;
        }

        /// <summary>
        /// When overridden by a derived class, provides error information from the current connection.
        /// </summary>
        /// <param name="message">Human-readable information about the error.</param>
        /// <param name="ex">An internal exception containing the error details.</param>
        protected virtual void OnError(string message, Exception ex)
        {
            Console.WriteLine(message);
            Console.WriteLine(ex.ToString());
        }

        /// <summary>
        /// Gets whether the connection is alive or not.
        /// </summary>
        public bool Connected { get { return m_open; } }
        private bool m_open = false;

        /// <summary>
        /// Closes the connection and prepares for a new connection.
        /// </summary>
        public virtual void Close()
        {
            if (m_open)
            {
                m_open = false;
                m_client.Close();
            }
        }

        /// <summary>
        /// Once the connection is established, gets the local endpoint to which the client is bound.
        /// </summary>
        public IPEndPoint LocalEP
        {
            get
            {
                return (IPEndPoint)m_client.Client.LocalEndPoint;
            }
        }

        public IPEndPoint RemoteEP
        {
            get
            {
                return (IPEndPoint)m_client.Client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// For derived classes, sends the specified binary data to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        protected virtual void Send(byte[] data)
        {
            m_client.GetStream().Write(data, 0, data.Length);
        }

        /// <summary>
        /// Sends the specified buffer to the server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public virtual void Send(DataBuffer data)
        {
            byte[] info = data.GetData();
            m_client.GetStream().Write(info, 0, info.Length);
        }

        /// <summary>
        /// Receives the specified number of bytes.
        /// </summary>
        /// <remarks>
        /// <para>This method blocks in a loop until all of the data comes through.  For that reason, it 
        /// is recommended that this method is only used in a background thread.</para>
        /// </remarks>
        /// <param name="len">The amount of data to receive.</param>
        /// <returns>A byte array containing the specified data.</returns>
        public virtual byte[] Receive(int len)
        {
            byte[] incBuffer = new byte[len];
            int totRecv = 0;
            if (!m_open)
                return null;

            NetworkStream localNS = m_client.GetStream();
            while (m_open
                && m_client.Connected
                && totRecv < len)
            {
                try
                {
                    totRecv += localNS.Read(incBuffer, totRecv, (int)len - totRecv);
                }
                catch (IOException se)
                {
                    Close();
                    OnError("A read error occurred on the connection.", se);
                }
            }

            return incBuffer;
        }

        /// <summary>
        /// Retrieves an arbitrarily-sized byte array of data.
        /// </summary>
        /// <returns>An array of bytes of data that have been received from the server.</returns>
        public virtual byte[] Receive()
        {
            byte[] incBuffer = new byte[2048];
            int recvsize = m_client.GetStream().Read(incBuffer, 0, 2048);
            byte[] result = new byte[recvsize];
            Buffer.BlockCopy(incBuffer, 0, result, 0, recvsize);
            return result;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the specified object.
        /// </summary>
        public virtual void Dispose()
        {
            if (m_client != null && m_client.Connected)
            {
                m_client.Close();
                m_client = null;
            }

            m_ipep = null;
            m_server = null;
            m_open = false;
        }

        #endregion
    }
}
