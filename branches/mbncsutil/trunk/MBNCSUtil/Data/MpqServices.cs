/*
MBNCSUtil -- Managed Battle.net Authentication Library
Copyright (C) 2005-2008 by Robert Paveza

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met: 

1.) Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. 
2.) Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution. 
3.) The name of the author may not be used to endorse or promote products derived 
from this software without specific prior written permission. 
	
See LICENSE.TXT that should have accompanied this software for full terms and 
conditions.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MBNCSUtil.Data
{
    /// <summary>
    /// Provides access to the loading and unloading of MPQ archives.  This class cannot be instantiated or inherited.
    /// </summary>
    public sealed class MpqServices
    {
        #region instance fields
        private string m_path;
        private IntPtr m_hMod;
        private List<MpqArchive> m_archives;
        #endregion
        #region lazy singleton
        private class SingletonHost
        {
            public static MpqServices Singleton = new MpqServices();
            static SingletonHost() { }
        }
        private static MpqServices Instance { get { return SingletonHost.Singleton; } }

        private MpqServices()
        {
            m_path = Path.GetTempFileName();
            Console.WriteLine(m_path);
            FileStream fs = new FileStream(m_path, FileMode.Open, FileAccess.Write, FileShare.None);
            byte[] storm_dll;
            if (Native.Is64BitProcess)
                storm_dll = Resources.StormLib64;
            else
                storm_dll = Resources.StormLib32;

            fs.Write(storm_dll, 0, storm_dll.Length);
            fs.Close();

            m_hMod = Native.LoadLibrary(m_path);
            if (m_hMod == IntPtr.Zero)
            {
                File.Delete(m_path);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            LateBoundStormDllApi.Initialize(m_hMod);

            m_archives = new List<MpqArchive>();
        }
        #endregion

        /// <summary>
        /// Opens an MPQ archive at the specified path.
        /// </summary>
        /// <param name="fullPath">The path to the MPQ archive.</param>
        /// <returns>An <see cref="MpqArchive">MpqArchive</see> instance representing the archive.</returns>
        /// <exception cref="MpqException">Thrown if there is an error with the MPQ archive.</exception>
        public static MpqArchive OpenArchive(string fullPath)
        {
            object o = Instance;

            MpqArchive arch = new MpqArchive(fullPath);
            Instance.m_archives.Add(arch);

            return arch;
        }

        internal static void NotifyArchiveDisposed(MpqArchive archive)
        {
            if (Instance.m_archives.Contains(archive))
                Instance.m_archives.Remove(archive);
        }

        /// <summary>
        /// Closes an MPQ archive.
        /// </summary>
        /// <param name="archive">The archive to close.</param>
        public static void CloseArchive(MpqArchive archive)
        {
            object o = Instance;

            archive.Dispose();
        }
    }
}
