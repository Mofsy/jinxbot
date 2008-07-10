using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BNSharp;

// NOTE: If you change the interface name "IJinxBotWebApplication" here, you must also update the reference to "IJinxBotWebApplication" in Web.config.
[ServiceContract]
public interface IJinxBotWebApplication
{
    [OperationContract(IsOneWay = true)]
    void PostEvent(BaseEventArgs args);
}
