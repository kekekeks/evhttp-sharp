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
            if (req.Uri == null)
            {
                req.Respond(System.Net.HttpStatusCode.BadRequest, new Dictionary<string, string>(), new byte[0]);
                return;
            }
            ThreadPool.QueueUserWorkItem(_ =>
            {
                PreProcessRequest(req);
                var pairs = req.Uri.Split(new[] {'?'}, 2);
                var path = Uri.UnescapeDataString(pairs[0]);
                var query = pairs.Length == 2 ? pairs[1] : string.Empty;
                var nreq = CreateRequest(req.Method, path, req.Headers,
                    RequestStream.FromStream(new MemoryStream(req.RequestBody)), "http", query, req.UserHostAddress);
                try
                {
                    _engine.HandleRequest(
                        nreq,
                        ctx =>
                        {
                            ResponseData resp;
                            try
                            {
                                var ms = new MemoryStream();
                                PostProcessNancyResponse(nreq, ctx.Response);
                                ctx.Response.Contents(ms);
                                resp = new ResponseData(ctx.Response.StatusCode, ms.ToArray(), ctx.Response.Headers);

                            }
                            catch (Exception e)
                            {
                                resp = GetExceptionResponse(e);
                            }
                            DoRespond(req, resp);
                        },
                        exception => DoRespond(req, GetExceptionResponse(exception)));
                }
                catch (Exception e)
                {
                    DoRespond(req, GetExceptionResponse(e));
                }
            });
        }

        void DoRespond(EventHttpRequest req, ResponseData resp)
        {
            req.Respond((System.Net.HttpStatusCode) resp.Code, resp.Headers ?? new Dictionary<string, string>(),
                resp.Data ?? new byte[0]);
        }


        public class ResponseData
        {
            public ResponseData(HttpStatusCode code, byte[] data, IDictionary<string, string> headers)
            {
                Headers = headers;
                Data = data;
                Code = code;
            }

            public HttpStatusCode Code { get; private set; }
            public IDictionary<string, string> Headers { get; private set; }
            public byte[] Data { get; private set; }

        }

        protected virtual ResponseData GetExceptionResponse(Exception e)
        {
            
            if (Error != null)
                Error(this, new ErrorEventArgs(e));
            else
                Console.WriteLine(e);
            return new ResponseData(HttpStatusCode.InternalServerError, Encoding.UTF8.GetBytes(e.ToString()),
                new Dictionary<string, string>
                {
                    {"Content-Type", "text/plain; charset=utf-8"}
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
