using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
	class EvHttpBoundSocket : SafeHandle
	{
		public EvHttpBoundSocket() : base(IntPtr.Zero, false)
		{
		}

		protected override bool ReleaseHandle()
		{
			throw new InvalidOperationException("This handle can't be released from user code");
		}

		public override bool IsInvalid
		{
			get { return handle == IntPtr.Zero; }
		}
	}
}
