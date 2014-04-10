using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public string UserHostAddress { get; set; }
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
            IEnumerable<string> host;
            if (Headers.TryGetValue("Host", out host))
                Host = host.First().Split(':')[0];

            var evBuffer = new EvBuffer(Event.EvHttpRequestGetInputBuffer(_handle), false);
            if (!evBuffer.IsInvalid)
            {
                var len = Event.EvBufferGetLength(evBuffer).ToInt32();
                RequestBody = new byte[len];
                Event.EvBufferRemove(evBuffer, RequestBody, new IntPtr(len));
            }

            var conn = Event.EvHttpRequestGetConnection(_handle);
            IntPtr pHostString = IntPtr.Zero;
            ushort port = 0;
            Event.EvHttpConnectionGetPeer(conn, ref pHostString, ref port);
            UserHostAddress = Marshal.PtrToStringAnsi(pHostString);
            _listener.IncreaseRequestCounter();
        }

        private static readonly Dictionary<HttpStatusCode, string> StatusCodes = Enum.GetValues(typeof (HttpStatusCode))
            .Cast<HttpStatusCode>().Distinct().ToDictionary(x => x, x => x.ToString());

        public void Respond(System.Net.HttpStatusCode code, IDictionary<string, string> headers,
            ArraySegment<byte>[] body)
        {
            var pHeaders = Event.EvHttpRequestGetOutputHeaders(_handle);
            foreach (var header in headers.Where(h => h.Key != "Content-Length"))
                Event.EvHttpAddHeader(pHeaders, header.Key, header.Value);
            Event.EvHttpAddHeader(pHeaders, "Content-Length", body.Sum(chunk => chunk.Count).ToString());
            var buffer = Event.EvBufferNew();
            foreach (var chunk in body)
            {
                Event.EvBufferAdd(buffer, chunk.Array, new IntPtr(chunk.Count));
            }
            string codeName;
            if (!StatusCodes.TryGetValue(code, out codeName))
                codeName = code.ToString();
            _listener.Sync(() =>
            {
                Event.EvHttpSendReply(_handle, (int) code, codeName, buffer);
                    buffer.Dispose();
                    _listener.DecreaseRequestCounter();
                });
        }


        public void Respond(System.Net.HttpStatusCode code, IDictionary<string, string> headers,
            IEnumerable<ArraySegment<byte>> body)
        {
            Respond(code, headers, body.ToArray());
        }

        public void Respond(System.Net.HttpStatusCode code, IDictionary<string, string> headers, byte[] body)
        {
            Respond(code, headers, new[] {new ArraySegment<byte>(body, 0, body.Length)});
        }

    }
}