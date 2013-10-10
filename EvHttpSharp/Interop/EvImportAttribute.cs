using System;

namespace EvHttpSharp.Interop
{
	internal class EvImportAttribute : Attribute
	{
		public EvDll Dll { get; set; }

		public EvImportAttribute(EvDll dll = EvDll.Core)
		{
			Dll = dll;
		}
	}

	internal enum EvDll
	{
		Core,
		Extra,
		Pthread
	}
}