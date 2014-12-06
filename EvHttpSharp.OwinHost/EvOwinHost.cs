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
    public static class EvOwinHost
    {
        public static IDisposable Start(string address, int port, Action<IAppBuilder> startup)
        {
            var opts = new StartOptions
            {
                Port = port,
                ServerFactory = "EvHttpSharp.OwinHost.EvOwinFactory"
            };
            EvOwinFactory.ThreadInit(address, port);
            return WebApp.Start(opts, startup);

        }



    }
}
