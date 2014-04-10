using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EvHttpSharp.Interop
{
    internal class EvConnListener : SafeHandle
    {
        public EvConnListener()
            : base(IntPtr.Zero, true)
        {
        }

        private bool _disowned;

        public void Disown()
        {
            _disowned = true;
        }

        protected override bool ReleaseHandle()
        {
            if (!_disowned)
                Event.EvConnListenerFree(handle);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public IntPtr FileDescriptor
        {
            get
            {
                return Platform.RunningOnWindows
                    ? Event.EvConnListenerGetFdWindows(this)
                    : new IntPtr(Event.EvConnListenerGetFdNix(this));
            }
        }
    }
}