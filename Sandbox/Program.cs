using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Http;
using EvHttpSharp;
using Nancy;
using Owin;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LibLocator.Init();
            var rawMode = args.Contains("raw");
            var owin = args.Contains("--owin");
            const string workerPrefix = "--workers=";
            var workers =
                args.Where(a => a.StartsWith(workerPrefix))
                    .Select(a => int.Parse(a.Substring(workerPrefix.Length)))
                    .FirstOrDefault();
            if (workers == 0)
                workers = 1;

            if (rawMode)
            {
                new EventHttpMultiworkerListener(
                    req =>
                        req.Respond(HttpStatusCode.OK, new Dictionary<string, string>(),
                            Encoding.UTF8.GetBytes("Hello from thread " + Thread.CurrentThread.ManagedThreadId)),
                    workers).Start(
                        args[0], ushort.Parse(args[1]));
            }
            else if (owin)
            {
                EvHttpSharp.OwinHost.EvOwinHost.Start(args[0], int.Parse(args[1]), builder =>
                {
                    var config = new HttpConfiguration();

                    builder.UseWebApi(config);
                    config.MapHttpAttributeRoutes();
                });
            }
            else 
            {
                var host = new Nancy.Hosting.Event2.NancyEvent2Host(args[0], int.Parse(args[1]),
                    new DefaultNancyBootstrapper(), workers);
                host.Start();
            }
        }

    }
}