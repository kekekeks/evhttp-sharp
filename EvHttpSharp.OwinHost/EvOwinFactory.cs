using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvHttpSharp.OwinHost
{
    static class EvOwinFactory
    {

        //HACK: dunno how to pass these correctly
        [ThreadStatic]
        private static string _host;

        [ThreadStatic]
        private static ushort _port;


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

        public static void ThreadInit(string address, int port)
        {
            _host = address;
            _port = (ushort)port;
        }
    }
}
