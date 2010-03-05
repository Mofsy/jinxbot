/*
Copyright (c) 2008 Oleg Mihailik

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace JinxBot.WebProtocols
{
	internal sealed class TraceHelper
	{
		private TraceHelper(){}

        [Conditional("TRACE")]
        public static void TraceException(object @this, Exception err)
        {
            //MethodBase method=new StackFrame(1,false).GetMethod();
            //StringBuilder output=new StringBuilder();
            //output.Append("!! ");
            //output.Append( method.ReflectedType.Name );
            //if( !method.IsStatic )
            //{
            //    output.Append("#");
            //    if( @this==null )
            //        output.Append("null");
            //    else
            //        output.Append(@this.GetHashCode());
            //    output.Append(":");
            //}
            //output.Append(err.ToString());
            //Trace.WriteLine(output.ToString());
        }

        [Conditional("TRACE")]
        public static void TraceMethod(object @this, params object[] argValues)
        {
            //try
            //{
            //    StackTrace callStack=new StackTrace(1,false);
            //    MethodBase method=callStack.GetFrame(0).GetMethod();
            //    StringBuilder output=new StringBuilder();

            //    output.Append(">");
            //    output.Append(new string('>',callStack.FrameCount));
            //    output.Append(" ");
            
            //    if( @this==null )
            //    {
            //        output.Append( method.ReflectedType.Name );
            //        output.Append(": ");
            //        output.Append( @this );
            //    }
            //    else
            //    {

            //        output.Append( method.ReflectedType.Name );
            
            //        if( !method.IsStatic )
            //        {
            //            output.Append("#");
            //            if( @this==null )
            //                output.Append("null");
            //            else
            //                output.Append(@this.GetHashCode());

            //            output.Append(".");

            //            // for interface methods -- get only interface and method simple names
            //            // (skip full namespaces)
            //            if( method is MethodInfo )
            //            {
            //                int lastDot=method.Name.LastIndexOf(".");

            //                if( lastDot>0 )
            //                {
            //                    string methodName=method.Name.Substring(lastDot+1);
            //                    string intfName=method.Name.Substring(0,lastDot);
            //                    lastDot=intfName.LastIndexOf(".");
            //                    if( lastDot>0 )
            //                        intfName=intfName.Substring(lastDot+1);

            //                    output.Append(intfName);
            //                    output.Append(".");
            //                    output.Append(methodName);
            //                }
            //                else
            //                    output.Append( method.Name );
            //            }
            //            else
            //                output.Append(method.Name);
            //        }
            //        else
            //        {
            //            output.Append(".");
            //            output.Append(method.Name);
            //        }

            //        output.Append("(");
            
            //        ParameterInfo[] args=method.GetParameters();
            //        int skipOutParams=0;
            //        if( args!=null && args.Length!=0 )
            //        {
            //            for( int i=0; i<args.Length; i++ )
            //            {
            //                if( i!=0 )
            //                    output.Append(", ");
            //                else
            //                    output.Append(" ");

            //                if( ( args[i].IsOut || args[i].IsRetval ) //|| args[i].ParameterType.IsByRef +? )
            //                    && !args[i].IsIn )
            //                {
            //                    output.Append(args[i].Name);
            //                    output.Append(": ");
            //                    if( args[i].IsRetval )
            //                        output.Append("retval");
            //                    else
            //                        output.Append("out");
            //                    skipOutParams++;
            //                }
            //                else
            //                {
            //                    if( argValues==null || i>=argValues.Length )
            //                        WriteArgument(output,args[i]);
            //                    else
            //                        WriteArgument(output,args[i],argValues[i-skipOutParams]);
            //                }

            //                if( i==args.Length-1 )
            //                    output.Append(" ");
            //            }
            //        }

            //        output.Append(")");

            //    }

            //    output.Append(" // Thread#");
            //    output.Append(Thread.CurrentThread.GetHashCode());


            //    Trace.WriteLine(output.ToString());
            //}
            //catch( Exception traceException )
            //{
            //    Trace.WriteLine(
            //        "What? "+traceException );
            //}
        }

        static void WriteArgument(StringBuilder output, ParameterInfo arg)
        {
            //output.Append(arg.ParameterType);
            ////output.Append(": undefined");
        }

        static void WriteArgument(StringBuilder output, ParameterInfo arg, object value)
        {
            //output.Append(arg.Name);
            //output.Append(": ");

            //if( value==null )
            //{
            //    output.Append("null");
            //    return;
            //}

            //if( value is string && arg.ParameterType!=typeof(string) )
            //    output.Append(value as string);
            //else 
            //{
            //    if( arg.ParameterType.IsByRef )
            //        output.Append("ref ");
            //    output.Append(ObjectValue(value));
            //}
        }
	}
}
