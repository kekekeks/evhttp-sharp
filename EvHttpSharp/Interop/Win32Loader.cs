using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
    internal class Win32Loader : IDynLoader
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", EntryPoint = "LoadLibraryW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpszLib);

        IntPtr IDynLoader.LoadLibrary(string basePath, string dll)
        {
            dll += "-2-0-5.dll";
            if (basePath != null)
                dll = System.IO.Path.GetFullPath(System.IO.Path.Combine(basePath, dll));
            var handle = LoadLibrary(dll);
            if (handle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return handle;
        }

        IntPtr IDynLoader.GetProcAddress(IntPtr dll, string proc)
        {
            var ptr = GetProcAddress(dll, proc);
            if (ptr == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return ptr;
        }
    }
}