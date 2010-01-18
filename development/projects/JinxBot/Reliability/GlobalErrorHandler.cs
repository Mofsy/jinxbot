using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Views;
using BNSharp;
using System.Globalization;

namespace JinxBot.Reliability
{
    internal static class GlobalErrorHandler
    {
        public static ErrorLogDocument ErrorLog = new ErrorLogDocument();

        public static void ReportException(Exception ex)
        {
            ErrorLog.AddError(ex.ToString());
        }

        public static void ReportEventException(string profileName, EventExceptionEventArgs args)
        {
            try
            {
                ErrorLog.AddError(string.Format(CultureInfo.CurrentCulture, "Error in {0}:", profileName));
                ErrorLog.AddError(args.ToString("v"));
            }
            catch { }
        }

        public static void Initialize()
        {
            // make sure type gets initialized, that's all...
        }
    }
}
