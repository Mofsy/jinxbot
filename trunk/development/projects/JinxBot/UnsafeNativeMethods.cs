using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace JinxBot
{
    internal static class UnsafeNativeMethods
    {
        [DllImport("User32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll", EntryPoint = "UpdateWindow", CharSet = CharSet.Auto)]
        internal static extern bool UpdateWindow(IntPtr hWnd);
    }
}
