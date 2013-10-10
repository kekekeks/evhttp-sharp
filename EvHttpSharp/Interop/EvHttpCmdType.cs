using System;

namespace EvHttpSharp.Interop
{
	[Flags]
	internal enum EvHttpCmdType
	{
		Get = 1 << 0,
		Post = 1 << 1,
		Head = 1 << 2,
		Put = 1 << 3,
		Delete = 1 << 4,
		Options = 1 << 5,
		Trace = 1 << 6,
		Connect = 1 << 7,
		Patch = 1 << 8,
		All = Get | Post | Head | Put | Delete | Options | Trace | Connect | Patch
	}
}
