using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;

namespace Sandbox
{
	public class AsyncModule : NancyModule
	{
		public AsyncModule()
		{
			Get["async", true] = (args, token) =>
				{
					var tcs = new TaskCompletionSource<object>();

					ThreadPool.QueueUserWorkItem(_ =>
						{
							Thread.Sleep(100);
							tcs.SetResult("OK");
						});

					return tcs.Task;
				};
		}
	}
}
