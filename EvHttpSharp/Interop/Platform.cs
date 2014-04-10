using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EvHttpSharp.Interop
{
    static class Platform
    {
        public static readonly bool RunningOnWindows = Path.DirectorySeparatorChar == '\\';

        [DllImport("libc", EntryPoint = "close")]
        private static extern void Close(int fd);

        [DllImport("kernel32.dll")]
        private static extern void CloseHandle(IntPtr hFile);

        public static void CloseFileDescriptor(IntPtr hFile)
        {
            if (RunningOnWindows)
                CloseHandle(hFile);
            else
                Close(hFile.ToInt32());
        }


    }
}
