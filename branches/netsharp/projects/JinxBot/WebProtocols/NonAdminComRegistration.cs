/*
Copyright (c) 2008 Oleg Mihailik

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace JinxBot.WebProtocols
{
    internal static class NonAdminComRegistration
    {
        public static void Unregister<T>()
            where T : new()
        {
            string clsid = typeof(T).GUID.ToString("b");
            
            string clsidKeyPath = @"Software\Classes\CLSID\" + typeof(T).GUID.ToString("B");
            Registry.CurrentUser.DeleteSubKeyTree(clsidKeyPath);

            string progIdPath = @"Software\Classes\" + typeof(T).FullName;
            Registry.CurrentUser.DeleteSubKeyTree(progIdPath);
        }

        public static void Register<T>()
            where T : new()
        {
            string clsid = typeof(T).GUID.ToString("b");
            string clsidKeyPath = @"Software\Classes\CLSID\" + typeof(T).GUID.ToString("B");
            using (RegistryKey clsidKey = Registry.CurrentUser.CreateSubKey(clsidKeyPath))
            {
                clsidKey.SetValue(null, typeof(T).FullName);

                using (RegistryKey inproc32Key = clsidKey.CreateSubKey("InprocServer32"))
                {
                    inproc32Key.SetValue(null, "mscoree.dll");

                    inproc32Key.SetValue("Assembly", typeof(T).Assembly.FullName);
                    inproc32Key.SetValue("Class", typeof(T).FullName);
                    inproc32Key.SetValue("RuntimeVersion", typeof(T).Assembly.ImageRuntimeVersion);
                    inproc32Key.SetValue("ThreadingModel", "Both");
                }

                using (RegistryKey implementedCategoriesKey = clsidKey.CreateSubKey("Implemented Categories"))
                {
                    implementedCategoriesKey.CreateSubKey("{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}");
                }

                using (RegistryKey progIdKey = clsidKey.CreateSubKey("ProgId"))
                {
                    progIdKey.SetValue(null, typeof(T).FullName);
                }
            }

            string progIdPath = @"Software\Classes\" + typeof(T).FullName;
            using (RegistryKey progIdKey = Registry.CurrentUser.CreateSubKey(progIdPath))
            {
                using (RegistryKey clsidKey = progIdKey.CreateSubKey("CLSID"))
                {
                    clsidKey.SetValue(null, clsid);
                }
            }
        }
    }
}
