using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
	class EvBuffer  : SafeHandle
	{
		public EvBuffer()
			: base(IntPtr.Zero, true)
		{
			
		}

		public EvBuffer(IntPtr h, bool owned) : base(IntPtr.Zero, owned)
		{
			handle = h;
		}

		protected override bool ReleaseHandle()
		{
			Event.EvBufferFree(handle);
			return true;
		}

		public override bool IsInvalid
		{
			get { return handle == IntPtr.Zero; }
		}
	}
}
