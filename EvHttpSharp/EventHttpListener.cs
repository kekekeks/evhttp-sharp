using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using EvHttpSharp.Interop;

namespace EvHttpSharp
{
	public class EventHttpListener : IDisposable
	{
		private readonly RequestCallback _cb;
		public delegate void RequestCallback(EventHttpRequest req);

		private EventBase _eventBase;
		private EvHttp _evHttp;
		private Thread _thread;
		private GCHandle _httpCallbackHandle;
		private GCHandle _syncCbHandle;
		private EvEvent _syncCbEvent;
		private readonly Queue<Action> _syncCallbacks = new Queue<Action>();
		private bool _stop;

		public EventHttpListener(RequestCallback cb)
		{
			LibLocator.TryToLoadDefaultIfNotInitialized();
			_cb = cb;
		}

		public void Start(string host, ushort port)
		{
			_eventBase = Event.EventBaseNew();
			if (_eventBase.IsInvalid)
				throw new IOException("Unable to create event_base");
			_evHttp = Event.EvHttpNew(_eventBase);
			if (_evHttp.IsInvalid)
			{
				Dispose();
				throw new IOException ("Unable to create evhttp");
			}
			var socket = Event.EvHttpBindSocketWithHandle(_evHttp, host, port);
			if (socket.IsInvalid)
			{
				Dispose();
				throw new IOException("Unable to bind to the specified address");
			}

			_thread = new Thread(MainCycle);
			_thread.Start();
		}

		private void MainCycle()
		{
			var cb = new Event.D.evhttp_request_callback (RequestHandler);
			_httpCallbackHandle = GCHandle.Alloc (cb);
			Event.EvHttpSetAllowedMethods (_evHttp, EvHttpCmdType.All);
			Event.EvHttpSetGenCb (_evHttp, cb, GCHandle.ToIntPtr (_httpCallbackHandle));

			var syncCb = new Event.D.event_callback (SyncCallback);
			_syncCbHandle = GCHandle.Alloc (syncCb);
			using (_syncCbEvent = Event.EventNew(_eventBase, -1, 0, syncCb, IntPtr.Zero))
			{
				while (!_stop)
				{
					Event.EventBaseDispatch(_eventBase);
				}
			}
			//We've recieved loopbreak from actual Dispose, so dispose now
			DoDispose ();
			_httpCallbackHandle.Free ();
			_syncCbHandle.Free ();
		}

		private void SyncCallback(int fd, short events, IntPtr arg)
		{
			lock (_syncCallbacks)
				while (_syncCallbacks.Count != 0)
					_syncCallbacks.Dequeue () ();
		}

		private void RequestHandler(IntPtr request, IntPtr arg)
		{
			var req = new EventHttpRequest (this, request);
			_cb (req);
		}

		internal void Sync(Action cb)
		{
			lock (_syncCallbacks)
				_syncCallbacks.Enqueue(cb);
			Event.EventActive(_syncCbEvent);
		}

		private void DoDispose()
		{

			if (_eventBase != null && !_eventBase.IsInvalid)
				_eventBase.Dispose();
			if (_evHttp != null && !_evHttp.IsInvalid)
				_evHttp.Dispose();
		}

		public void Dispose()
		{
			if (_thread == null)
				DoDispose();
			else if (_eventBase != null && !_eventBase.IsClosed)
			{
				_stop = true;
				if (_thread != Thread.CurrentThread)
					_thread.Join ();
			}

		}
	}
}
