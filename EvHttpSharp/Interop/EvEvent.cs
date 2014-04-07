using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
    class EvEvent : SafeHandle
    {
        public EvEvent() : base(IntPtr.Zero, true)
        {
            
        }

        protected override bool ReleaseHandle()
        {
            Event.EventFree(handle);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }
}
