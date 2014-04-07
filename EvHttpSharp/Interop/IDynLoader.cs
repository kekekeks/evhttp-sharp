using System;

namespace EvHttpSharp.Interop
{
    internal interface IDynLoader
    {
        IntPtr LoadLibrary(string basePath, string dll);
        IntPtr GetProcAddress(IntPtr dll, string proc);

    }
}