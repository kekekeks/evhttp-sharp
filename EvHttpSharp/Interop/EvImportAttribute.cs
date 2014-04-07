using System;

namespace EvHttpSharp.Interop
{
    internal class EvImportAttribute : Attribute
    {
        public EvDll Dll { get; set; }
        public string Name { get; set; }

        public EvImportAttribute(EvDll dll = EvDll.Core, string name = null)
        {
            Dll = dll;
            Name = name;
        }
    }

    internal enum EvDll
    {
        Core,
        Extra,
        Pthread
    }
}