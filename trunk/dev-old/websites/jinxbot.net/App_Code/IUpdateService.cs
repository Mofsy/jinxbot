using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: If you change the interface name "IUpdateService" here, you must also update the reference to "IUpdateService" in Web.config.
[ServiceContract]
public interface IUpdateService
{
    [OperationContract]
    void DoWork();
}
