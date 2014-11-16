using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace EvHttpSharp.OwinHost
{
    public sealed class EvOwinHost : IDisposable
    {
        public static IDisposable Start(string address, int port, Action<IAppBuilder> startup)
        {
            var opts = new StartOptions
            {
                Port = port,
                ServerFactory = "EvHttpSharp.OwinHost.EvOwinHost"
            };
            _host = address;
            _port = (ushort) port;
            return WebApp.Start(opts, startup);

        }

        //HACK: dunno how to pass these correctly
        [ThreadStatic] private static string _host;

        [ThreadStatic] private static ushort _port;

        public static void Initialize(IDictionary<string, object> properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");
            properties["owin.Version"] = "1.0";
        }



        public static IDisposable Create(Func<IDictionary<string, object>, Task> app,
            IDictionary<string, object> properties)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (properties == null)
                throw new ArgumentNullException("properties");

            return new EvOwinListener(app, _host, _port);
        }

        private EvOwinHost()
        {
            
        }

        public void Dispose()
        {
           
        }
    }
}
