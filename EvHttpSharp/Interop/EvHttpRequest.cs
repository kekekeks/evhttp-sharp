using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
	class EvHttpRequest : SafeHandle
	{
		public EvHttpRequest(IntPtr h) : base(IntPtr.Zero, false)
		{
			handle = h;
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
