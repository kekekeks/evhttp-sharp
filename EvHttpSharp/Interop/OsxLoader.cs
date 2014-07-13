using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
    class OsxLoader : IDynLoader
    {
// ReSharper disable InconsistentNaming
        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern IntPtr dlerror();
// ReSharper restore InconsistentNaming

        static string DlError()
        {
            return Marshal.PtrToStringAuto(dlerror());
        }

        public IntPtr LoadLibrary(string basePath, string dll)
        {
            dll += "-2.0.5.dylib";

            if (basePath != null)
                dll = System.IO.Path.Combine(basePath, dll);
            var handle = dlopen(dll, 1);

            /* If your mono is 32bit but your dylib are 64bit (like the ones
             * in brew), you'll end up with an exception like:
             *
             * dlopen(/usr/local/Cellar/libevent/2.0.21/lib/libevent_core-2.0.5.dylib, 1): no suitable image found.  Did find:
             * /usr/local/Cellar/libevent/2.0.21/lib/libevent_core-2.0.5.dylib: mach-o, but wrong architecture
             *
             * The bit'ness of the binary can be found out like:
             * otool -h /usr/local/lib/libevent_pthreads-2.0.5.dylib
             * Magic of 0xfeedface means 32bit, 0xfeedfacf means 64bit.
             * 
             * To compile 32bit on 64bit OSX:
             * CFLAGS="-m32" ./configure && make && sudo make install
             */
            if (handle == IntPtr.Zero)
                throw new Win32Exception(DlError());
            return handle;
        }

        public IntPtr GetProcAddress(IntPtr dll, string proc)
        {
            var ptr = dlsym(dll, proc);
            if (ptr == IntPtr.Zero)
                throw new Win32Exception(DlError());
            return ptr;
        }
    }
}
