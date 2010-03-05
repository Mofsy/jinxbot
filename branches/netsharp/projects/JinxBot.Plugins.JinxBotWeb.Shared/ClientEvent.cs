using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BNSharp;

namespace JinxBot.Plugins.JinxBotWeb
{
    [DataContract]
    public class ClientEvent
    {
        [DataMember]
        public ClientEventType EventType { get; set; }

        [DataMember]
        public BaseEventArgs EventData { get; set; }
    }
}
