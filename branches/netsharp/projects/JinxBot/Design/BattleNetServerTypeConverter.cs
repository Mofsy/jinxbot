using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;

namespace JinxBot.Design
{
    [Obsolete]
    internal sealed class BattleNetServerTypeConverter : ServerTypeConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<Server> baseServers = new List<Server>
            {
                new Server("useast.battle.net", 6112),
                new Server("uswest.battle.net", 6112),
                new Server("europe.battle.net", 6112),
                new Server("asia.battle.net", 6112)
            };

            List<Server> serverCollection = new List<Server>();
            foreach (Server s in baseServers)
            {
                serverCollection.Add(s);
                try
                {
                    IPHostEntry iphe = Dns.GetHostEntry(s.Host);
                    foreach (IPAddress ipa in iphe.AddressList)
                    {
                        serverCollection.Add(new Server(ipa.ToString(), 6112));
                    }
                }
                catch
                {
                    Debug.WriteLine(s, "Unable to resolve server.");
                }
            }

            StandardValuesCollection svc = new StandardValuesCollection(serverCollection);
            return svc;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
