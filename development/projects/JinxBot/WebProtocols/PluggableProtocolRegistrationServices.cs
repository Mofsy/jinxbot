/*
Copyright (c) 2008 Oleg Mihailik

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Microsoft.Win32;

//using Microsoft.InternetExplorer.Urlmon.Interop;

namespace JinxBot.WebProtocols
{
	internal sealed class PluggableProtocolRegistrationServices
	{
        PluggableProtocolRegistrationServices() {}

        public static void RegisterPermanentProtocolHandler(Type protocolHandlerClass, string protocol)
        {
            if( protocolHandlerClass==null )
                throw new ArgumentNullException("protocolHandlerClass");

            CheckHandlerType(protocolHandlerClass);

            RegistryKey handlerKey=null;
            try
            {
                string keyPath="PROTOCOLS\\Handler\\"+protocol;
                try { handlerKey=Registry.ClassesRoot.OpenSubKey(keyPath,true); }
                catch {}
                if( handlerKey==null )
                    handlerKey=Registry.ClassesRoot.CreateSubKey(keyPath);

                using( handlerKey )
                {
                    //                    Console.WriteLine( handlerKey );

                    handlerKey.SetValue(
                        "CLSID",
                        protocolHandlerClass.GUID.ToString("B") );
                }
            }
            finally
            {
                if( handlerKey!=null )
                    handlerKey.Close();
            }
        }
    
        public static void UnregisterPermanentProtocolHandler(Type protocolHandlerClass, string protocol)
        {
            if( protocolHandlerClass==null )
                throw new ArgumentNullException("protocolHandlerClass");

            CheckHandlerType(protocolHandlerClass);

            RegistryKey handlerKey=null;
            try
            {
                string keyPath="PROTOCOLS\\Handler\\"+protocol;
                Registry.ClassesRoot.DeleteSubKey(keyPath,true);
            }
            finally
            {
                if( handlerKey!=null )
                    handlerKey.Close();
            }        
        }

        static Hashtable temporaryHandlerCFList=new Hashtable();

        struct HandlerAndProtocol
        {
            public Type ProtocolHandlerClass;
            public string Protocol;

            public HandlerAndProtocol(Type protocolHandler, string protocol)
            {
                this.ProtocolHandlerClass=protocolHandler;
                this.Protocol=protocol;
            }
        }

        public static void RegisterTemporaryProtocolHandler(Type protocolHandlerClass, string protocol)
        {
            if( protocolHandlerClass==null )
                throw new ArgumentNullException("protocolHandlerClass");

            lock( temporaryHandlerCFList )
            {
                HandlerAndProtocol token=new HandlerAndProtocol(protocolHandlerClass,protocol);

                if( temporaryHandlerCFList[token]!=null )
                    throw new InvalidOperationException(string.Format(
                        "Handler for this protocol already registered.", protocolHandlerClass.FullName ));


                CheckHandlerType(protocolHandlerClass);

                Guid handlerGuid=protocolHandlerClass.GUID;
                Guid iid_IUknown=NativeMethods.IID_IUnknown;
                Guid iid=NativeMethods.IID_IClassFactory;

                IntPtr handlerCF;
                int hResult=NativeMethods.DllGetClassObject(
                    ref handlerGuid,
                    ref iid,
                    out handlerCF );

                Marshal.ThrowExceptionForHR(hResult);

                if( handlerCF==IntPtr.Zero )
                {
                    RegistryKey inprocServer32=Registry.ClassesRoot.OpenSubKey(
                        @"CLSID\"+protocolHandlerClass.GUID.ToString("B")+@"\InprocServer32" );
                    if( inprocServer32==null
                        || (inprocServer32.GetValue(null)+"").ToLower()!="mscoree.dll"
                        || (inprocServer32.GetValue("Assembly")+"")==""
                        || (inprocServer32.GetValue("Class")+"")=="" )
                        throw new InvalidOperationException(
                            "Class seems to be not registered." );
                    else
                        throw new ExternalException(
                            "DllGetClassObject failed.", hResult );
                }

                string emptyStr=null;


                NativeMethods.IInternetSession session=GetSession();
                try
                {
                    session.RegisterNameSpace(
                        handlerCF,
                        ref handlerGuid,
                        protocol,
                        0,
                        ref emptyStr,
                        0 );
                }
                finally
                {
                    Marshal.ReleaseComObject(session);
                    session=null;
                }

                temporaryHandlerCFList[token]=handlerCF;
            }
        }

        public static void UnregisterTemporaryProtocolHandler(Type protocolHandlerClass,string protocol)
        {
            if( protocolHandlerClass==null )
                throw new ArgumentNullException("protocolHandlerClass");
            
            lock( temporaryHandlerCFList )
            {
                HandlerAndProtocol token=new HandlerAndProtocol(protocolHandlerClass,protocol);

                IntPtr handlerCF;
                if( temporaryHandlerCFList[token]==null )
                    throw new InvalidOperationException(string.Format(
                        "Protocol handler is not registered.", protocolHandlerClass.FullName ));
                else
                    handlerCF=(IntPtr)temporaryHandlerCFList[token];


                NativeMethods.IInternetSession session=GetSession();
                try
                {
                    session.UnregisterNameSpace(
                        handlerCF,
                        token.Protocol );
                }
                finally
                {
                    Marshal.ReleaseComObject(session);
                    session=null;
                }

                temporaryHandlerCFList[token]=null;
            }
        }

        static NativeMethods.IInternetSession GetSession()
        {
            NativeMethods.IInternetSession session;
            NativeMethods.CoInternetGetSession(
                IntPtr.Zero,
                out session,
                IntPtr.Zero );

            if( session==null )
                throw new InvalidOperationException("CoInternetGetSession failed with null result.");

            return session;
        }

        static void CheckHandlerType( Type handlerType )
        {
            //if( !typeof(IInternetProtocolRoot).IsAssignableFrom(handlerType) )
            //    throw new ArgumentException("HandlerType must implement IInternetProtocolRoot interface.");
            if( !typeof(NativeMethods.IInternetProtocol).IsAssignableFrom(handlerType) )
                throw new ArgumentException("HandlerType must implement IInternetProtocol interface.");
        }


    }
}
