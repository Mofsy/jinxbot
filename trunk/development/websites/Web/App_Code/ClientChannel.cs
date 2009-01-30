using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// Represents a channel available to a web client.  This class does not correspond to a Battle.net channel.
/// </summary>
[DataContract]
public class ClientChannel
{
    public ClientChannel(Channel channel)
    {
        ChannelID = channel.ChannelID;
        ClientName = channel.ClientName;
        Gateway = channel.Gateway;
    }

    [DataMember]
    public Guid ChannelID { get; set; }
    [DataMember]
    public string ClientName { get; set; }
    [DataMember]
    public string Gateway { get; set; }
}
