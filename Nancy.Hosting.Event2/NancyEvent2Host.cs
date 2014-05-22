using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvHttpSharp;
using Nancy.Bootstrapper;
using Nancy.IO;

namespace Nancy.Hosting.Event2
{
    public class NancyEvent2Host : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _workers;
        private readonly INancyEngine _engine;
        private IEventHttpListener _listener;

        public event ErrorEventHandler Error;

        public NancyEvent2Host(string host, int port, INancyBootstrapper bootstrapper, int workers)
        {
            _host = host;
            _port = port;
            _workers = workers;

            bootstrapper.Initialise();
            _engine = bootstrapper.GetEngine();
        }

        public NancyEvent2Host(string host, int port, INancyBootstrapper bootstrapper):this(host, port, bootstrapper, 1)
        {
            
        }

        public void Start()
        {
            _listener = _workers == 1
                ? (IEventHttpListener)new EventHttpListener(RequestHandler)
                : new EventHttpMultiworkerListener(RequestHandler, _workers);
            _listener.Start(_host, (ushort) _port);
        }

        public Task StopAsync()
        {
            if (_listener == null)
            {
                var tcs = new TaskCompletionSource<int>();
                tcs.SetResult(0);
                return tcs.Task;
            }
            return _listener.Shutdown().ContinueWith(_ => Dispose());
        }


        Request CreateRequest(string method, string path, IDictionary<string, IEnumerable<string>> headers, RequestStream body,
                              string scheme, string query = null, string ip = null)
        {
            return new Request(method, new Url {Path = path, Scheme = scheme, Query = query ?? String.Empty}, body, headers, ip);
        }

        private void RequestHandler(EventHttpRequest req)
        {
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    PreProcessRequest(req);
                    var pairs = req.Uri.Split(new[] {'?'}, 2);
                    var path = Uri.UnescapeDataString(pairs[0]);
                    var query = pairs.Length == 2 ? pairs[1] : string.Empty;
                    var nreq = CreateRequest(req.Method, path, req.Headers,
                                             RequestStream.FromStream(new MemoryStream(req.RequestBody)), "http", query, req.UserHostAddress);
                    _engine.HandleRequest(
                        nreq,
                        ctx =>
                            {
                                PostProcessNancyResponse(nreq, ctx.Response);

                                var ms = new MemoryStream();
                                ctx.Response.Contents(ms);
                                req.Respond((System.Net.HttpStatusCode) ctx.Response.StatusCode, ctx.Response.Headers, ms.ToArray());
                            },
                        exception =>
                            {
                                req.Respond(System.Net.HttpStatusCode.InternalServerError, new Dictionary<string, string>(), new byte[0]);
                                if (Error != null)
                                    Error(this, new ErrorEventArgs(exception));
                                else
                                    Console.WriteLine(exception);

                            });
                });
        }

        protected virtual void PreProcessRequest(EventHttpRequest request)
        {
            
        }

        protected virtual void PostProcessNancyResponse (Request request, Response response)
        {
            response.Headers["Content-Type"] = response.ContentType;
            response.Headers["Set-Cookie"] = string.Join("\r\n", response.Cookies.Select(cookie => cookie.ToString()));
        }

        public void Dispose()
        {
            _listener.Dispose ();
            _listener = null;
        }
    }
}
