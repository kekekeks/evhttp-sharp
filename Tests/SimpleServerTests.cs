using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using EvHttpSharp;
using Xunit;

namespace Tests
{
    public class SimpleServerTests : TestBase
    {
        private readonly ushort _freePort;
        private readonly string _urlBase;

        public SimpleServerTests()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            _freePort = (ushort) ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            _urlBase = "http://127.0.0.1:" + _freePort + "/";
        }


        private void CheckPortIsStillFree()
        {
            var l = new TcpListener(IPAddress.Loopback, _freePort);
            l.Start();
            l.Stop();
        }

        private void WithEachServer(Action<Func<EventHttpListener.RequestCallback, IEventHttpListener>> test)
        {
            test(cb => new EventHttpListener(cb));
            test(cb => new EventHttpMultiworkerListener(cb, 1));
            test(cb => new EventHttpMultiworkerListener(cb, 2));
        }

        [Fact]
        public void TestServerListening()
        {
            WithEachServer(listener =>
            {
                using (
                    var server =
                        listener(
                            r => r.Respond(System.Net.HttpStatusCode.OK, new Dictionary<string, string>(), new byte[0]))
                    )
                {
                    server.Start("127.0.0.1", _freePort);
                    var wc = new WebClient();

                    wc.DownloadData(_urlBase);
                }
                CheckPortIsStillFree();
            });
        }

        [Fact]
        public void ServerShouldStopListening()
        {
            WithEachServer(listener =>
            {

                using (
                    var server =
                        listener(
                            r => r.Respond(System.Net.HttpStatusCode.OK, new Dictionary<string, string>(), new byte[0]))
                    )
                {
                    server.Start("127.0.0.1", _freePort);
                    server.StopListeningAsync().Wait();
                    CheckPortIsStillFree();
                }
            });
        }

        [Fact]
        public void ServerShouldWaitForPendingRequests()
        {
            WithEachServer(listener =>
            {
                using (var server = listener(r => ThreadPool.QueueUserWorkItem(_ =>
                {
                    Thread.Sleep(1000);
                    r.Respond(HttpStatusCode.OK, new Dictionary<string, string>(), new byte[0]);
                })))
                {
                    server.Start("127.0.0.1", _freePort);

                    new WebClient().DownloadDataAsync(new Uri(_urlBase));
                    Thread.Sleep(100);
                    var task = server.Shutdown();
                    
                    Thread.Sleep(500);
                    Assert.False(task.IsCompleted);
                    Assert.True(task.Wait(4000));
                }
            });
        }

    }
}