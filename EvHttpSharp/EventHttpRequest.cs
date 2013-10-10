using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EvHttpSharp.Interop;

namespace EvHttpSharp
{
	public class EventHttpRequest
	{
		private readonly EventHttpListener _listener;
		private readonly EvHttpRequest _handle;
		public string Method { get; set; }
		public string Uri { get; set; }
		public string Host { get; set; }
		public IDictionary<string, IEnumerable<string>> Headers { get; set; }
		public byte[] RequestBody { get; set; }

		public EventHttpRequest(EventHttpListener listener, IntPtr handle)
		{
			_listener = listener;
			_handle = new EvHttpRequest(handle);
			Method = Event.EvHttpRequestGetCommand(_handle).ToString().ToUpper();
			Uri = Marshal.PtrToStringAnsi(Event.EvHttpRequestGetUri(_handle));
			var pHost = Event.EvHttpRequestGetHost(_handle);
			if (pHost != IntPtr.Zero)
				Host = Marshal.PtrToStringAnsi(pHost);
			Headers = EvKeyValuePair.ExtractDictinary(Event.EvHttpRequestGetInputHeaders(_handle));

			var evBuffer = new EvBuffer(Event.EvHttpRequestGetInputBuffer(_handle), false);
			if (!evBuffer.IsInvalid)
			{
				var len = Event.EvBufferGetLength(evBuffer).ToInt32();
				RequestBody = new byte[len];
				Event.EvBufferRemove(evBuffer, RequestBody, new IntPtr(len));
			}

		}


		public void Respond(System.Net.HttpStatusCode code, IDictionary<string, string> headers, byte[] body)
		{
			var pHeaders = Event.EvHttpRequestGetOutputHeaders(_handle);
			foreach (var header in headers)
				Event.EvHttpAddHeader(pHeaders, header.Key, header.Value);
			var buffer = Event.EvBufferNew();
			Event.EvBufferAdd(buffer, body, new IntPtr(body.Length));
			_listener.Sync(() =>
				{
					Event.EvHttpSendReply(_handle, (int) code, code.ToString(), buffer);
					buffer.Dispose();
				});
		}
	}
}