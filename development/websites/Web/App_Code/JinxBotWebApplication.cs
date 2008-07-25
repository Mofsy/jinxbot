using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BNSharp;
using System.Diagnostics;
using System.ServiceModel.Activation;

// NOTE: If you change the class name "JinxBotWebApplication" here, you must also update the reference to "JinxBotWebApplication" in Web.config.
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class JinxBotWebApplication : IJinxBotWebApplication
{
    public void PostEvent(Guid client, BaseEventArgs args)
    {
        Trace.WriteLine(args.ToString());

    }
}
