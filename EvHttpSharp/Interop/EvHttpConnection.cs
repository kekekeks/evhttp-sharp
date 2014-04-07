using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EvHttpSharp.Interop
{
    class EvHttpConnection : SafeHandle
    {
        public EvHttpConnection(IntPtr h) : base(IntPtr.Zero, false)
        {
            handle = h;
        }

        public EvHttpConnection() : this(IntPtr.Zero)
        {

        }

        protected override bool ReleaseHandle()
        {
            throw new NotSupportedException();
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }
}
