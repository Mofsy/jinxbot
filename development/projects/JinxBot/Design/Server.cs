using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace JinxBot.Design
{
    [Serializable]
    [TypeConverter(typeof(ServerTypeConverter))]
    [Obsolete]
    internal sealed class Server
    {
        public Server() { }
        public Server(string Hostname, int HostPort) { host = Hostname; port = HostPort; }

        private string host;
        private int port;
        [Description("Specifies the host URI")]
        public string Host { get { return host; } set { host = value; } }
        [Description("Specifies the host port")]
        public int Port { get { return port; } set { port = value; } }

        public override bool Equals(object obj)
        {
            if (obj is Server)
            {
                Server s = obj as Server;
                return host == s.host && port == s.port;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}:{1}", host, port);
        }
    }
}
