using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BNSharp.Net
{
    partial class BattleNetClient
    {
        partial void ReportException(Exception ex, params KeyValuePair<string, object>[] notes)
        {
            Trace.WriteLine(ex, "Unhandles Exception");
            foreach (KeyValuePair<string, object> item in notes)
            {
                Delegate del = item.Value as Delegate;
                if (del != null)
                {
                    Trace.WriteLine(item.Value, "(delegate type)");
                    Trace.WriteLine(del.Target, "-(delegate target)");
                    Trace.WriteLine(del.Method.Name, "-(delegate method name)");
                    continue;
                }

                Trace.WriteLine(item.Value, item.Key);
            }
        }
    }
}
