using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvHttpSharp.OwinHost
{
    class EvOwinListener : IDisposable
    {
        private readonly Func<IDictionary<string, object>, Task> _app;
        private readonly EventHttpListener _listener;
        private bool _disposed = false;

        public EvOwinListener(Func<IDictionary<string, object>, Task> app, string address, ushort port)
        {
            _app = app;
            _listener = new EventHttpListener(HandleRequest);
            _listener.Start(address, port);
        }

        private void HandleRequest(EventHttpRequest req)
        {
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                try
                {
                    var env = new Dictionary<string, object>();
                    env["owin.RequestBody"] = new MemoryStream(req.RequestBody);
                    env["owin.RequestHeaders"] = req.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray());
                    env["owin.RequestMethod"] = req.Method;

                    var pairs = req.Uri.Split(new[] { '?' }, 2);
                    var path = Uri.UnescapeDataString(pairs[0]);
                    var query = pairs.Length == 2 ? pairs[1] : string.Empty;
                    env["owin.RequestPath"] = path;
                    env["owin.RequestPathBase"] = "/";
                    env["owin.RequestProtocol"] = "HTTP/1.0";
                    env["owin.RequestQueryString"] = query;
                    env["owin.RequestScheme"] = "http";

                    var response = new MemoryStream();
                    var headers = new Dictionary<string, string[]>();
                    env["owin.ResponseBody"] = response;
                    env["owin.ResponseHeaders"] = headers;
                    env["owin.ResponseStatusCode"] = 200;

                    await _app(env);
                    req.Respond((HttpStatusCode) (int) env["owin.ResponseStatusCode"],
                        headers.Where(k => k.Value.Length != 0).ToDictionary(x => x.Key, x => x.Value[0]),
                        response.ToArray());

                }
                catch (Exception e)
                {
                    req.Respond(HttpStatusCode.InternalServerError, new Dictionary<string, string>(),
                        Encoding.UTF8.GetBytes(e.ToString()));
                }

                

            });
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _listener.Shutdown().Wait();
            _listener.Dispose();
        }
    }
}
