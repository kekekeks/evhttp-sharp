evhttp-sharp
============

libevent2-based HTTP server for C# with host for NancyFx

How to use
----------

At first, optionally call `LibLocator.Init("path-to-your-dll-directory")`, if you don't do that, it will use default system search order.

To use NancyFx host: `new Nancy.Hosting.Event2.NancyEvent2Host("127.0.0.1", 8081), new DefaultNancyBootstrapper()).Start();`

EventHttpListener example:
```csharp

    new EventHttpListener(req => 
    {
        req.Respond (HttpStatusCodes.OK, new Dictionary<string, string> { {"Content-Type", "text/plain" }}, Encoding.UTF8.GetBytes("Hello world"));
    }).Start("127.0.0.1", (ushort) 8081); 
```


Native binaries
---------------

### Windows

Precompiled binaries are in this repository, Visual Studio should copy them to build target location.

### Linux

It needs libevent_core, libevent_extra, libevent_pthreads installed in your system (or just use LibLocator.Init with path to binaries). In Debian-based distros you can install them using `apt-get install libevent-core-2.0-5 libevent-extra-2.0-5 libevent-core-2.0-5`
