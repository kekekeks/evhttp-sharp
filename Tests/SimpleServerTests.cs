using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
			_freePort = (ushort)((IPEndPoint) l.LocalEndpoint).Port;
			l.Stop();
			_urlBase = "http://127.0.0.1:" + _freePort + "/";
		}


		void CheckPortIsStillFree()
		{
			var l = new TcpListener(IPAddress.Loopback, _freePort);
			l.Start();
			l.Stop();
		}

		[Fact]
		public void TestServerListening()
		{
			using (
				var server =
					new EventHttpListener(r => r.Respond(System.Net.HttpStatusCode.OK, new Dictionary<string, string>(), new byte[0])))
			{
				server.Start("127.0.0.1", _freePort);
				var wc = new WebClient();
				wc.DownloadData(_urlBase);
			}
			CheckPortIsStillFree();
		}


	}
}
