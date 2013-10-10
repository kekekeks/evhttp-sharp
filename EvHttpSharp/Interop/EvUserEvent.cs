using System;
using System.Runtime.InteropServices;

namespace EvHttpSharp.Interop
{
	internal class EvUserEvent : IDisposable
	{
		public event EventHandler Triggered = delegate { };

		private readonly EvEvent _event;
		private GCHandle _nixCbHandle, _winCbHandle;

		public EvUserEvent(EventBase evBase)
		{
			if (Event.RunningOnWindows)
			{
				var winCb = new Event.D.event_callback_windows(CallbackWin);
				_winCbHandle = GCHandle.Alloc(winCb);
				_event = Event.EventNewWindows(evBase, new IntPtr(-1), 0, winCb, IntPtr.Zero);
			}
			else
			{
				var nixCb = new Event.D.event_callback_normal(CallbackNix);
				_nixCbHandle = GCHandle.Alloc(nixCb);
				_event = Event.EventNewNix(evBase, -1, 0, nixCb, IntPtr.Zero);
			}
		}

		private void CallbackWin(IntPtr fd, short events, IntPtr arg)
		{
			Triggered(this, new EventArgs());
		}

		private void CallbackNix(int fd, short events, IntPtr arg)
		{
			Triggered(this, new EventArgs());
		}

		public void Dispose()
		{
			if (_nixCbHandle.IsAllocated)
				_nixCbHandle.Free();
			if (_winCbHandle.IsAllocated)
				_winCbHandle.Free();
			if (_event != null)
				_event.Dispose();
		}

		public static implicit operator EvEvent(EvUserEvent ev)
		{
			return ev._event;
		}
	}
}