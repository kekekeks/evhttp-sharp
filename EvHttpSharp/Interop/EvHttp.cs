using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
	class EvHttp : SafeHandle
	{
		public EvHttp()
			: base(IntPtr.Zero, true)
		{

		}

		protected override bool ReleaseHandle()
		{
			Event.EvHttpFree(handle);
			return true;
		}

		public override bool IsInvalid
		{
			get { return IntPtr.Zero == handle; }
		}
	}
}
