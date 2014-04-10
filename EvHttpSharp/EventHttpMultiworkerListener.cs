using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EvHttpSharp.Interop;

namespace EvHttpSharp
{
    public class EventHttpMultiworkerListener : IEventHttpListener
    {
        private readonly List<EventHttpListener> _workers = new List<EventHttpListener>();
        private readonly EventHttpListener.RequestCallback _cb;
        private readonly int _workerCount;
        private IntPtr _ownedFileDescriptor;


        public EventHttpMultiworkerListener(EventHttpListener.RequestCallback cb, int workers)
        {
            if (workers < 1)
                throw new ArgumentException("Invalid number of workers");
            _cb = cb;
            _workerCount = workers;
        }

        public void Dispose()
        {
            _workers.ForEach(x => x.Dispose());
            if (_ownedFileDescriptor != IntPtr.Zero)
            {
                Platform.CloseFileDescriptor(_ownedFileDescriptor);
                _ownedFileDescriptor = IntPtr.Zero;
            }
        }

        void CheckAlreadyListening()
        {
            if (_workers.Count != 0)
                throw new InvalidOperationException("Already listening");
        }

        public void Start(string host, ushort port)
        {
            var soaddr = new Event.sockaddr_in
            {
                sin_family = Event.AF_INET,
                sin_port = (ushort)IPAddress.HostToNetworkOrder((short)port),
                sin_addr = 0,
                sin_zero = new byte[8]
            };

            using (var evBase = Event.EventBaseNew())
            using (
                var listener = Event.EvConnListenerNewBind(evBase, IntPtr.Zero, IntPtr.Zero, 1u << 3, 256, ref soaddr,
                    Marshal.SizeOf(soaddr)))
            {
                if (listener.IsInvalid)
                    throw new IOException("Unable to bind socket");
                _ownedFileDescriptor = listener.FileDescriptor;
            }
            Start(_ownedFileDescriptor);

        }

        public void Start(IntPtr sharedSocket)
        {
            _workers.AddRange(Enumerable.Repeat(0, _workerCount).Select(_ => new EventHttpListener(_cb)));
            _workers.ForEach(w => w.Start(sharedSocket));
        }

        private Task Aggregate(IEnumerable<Task> tasks)
        {
            var ta = tasks.ToArray();
            return Task.Factory.ContinueWhenAll(ta.ToArray(), results =>
            {
                var exceptions = results.Where(r => r.IsFaulted).Select(r => r.Exception).Cast<Exception>().ToArray();
                if (exceptions.Length != 0)
                    throw new AggregateException(exceptions);
            });
        }

        public Task StopListeningAsync()
        {
            return Aggregate(_workers.Select(x => x.StopListeningAsync()));
        }

        public Task WaitForPendingConnections()
        {
            return Aggregate(_workers.Select(x => x.WaitForPendingConnections()));
        }

        public Task Shutdown()
        {
            return Aggregate(_workers.Select(w => w.Shutdown()));
        }
    }
}
