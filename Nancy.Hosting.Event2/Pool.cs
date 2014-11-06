using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nancy.Hosting.Event2
{
    internal interface IPool
    {
        void Enqueue(Action act);
    }

    internal class SystemPool : IPool
    {
        public void Enqueue(Action act)
        {
            ThreadPool.QueueUserWorkItem(_ => act());
        }
    }


    internal class Pool : IPool
    {
        private readonly int _maximum;
        private readonly int _minimum;
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent _event = new AutoResetEvent(false);
        private long _numFree;
        private long _numTotal;

        public long Free { get { return _numFree; } }
        public long Total { get { return _numTotal; } }

        public Pool(int maximum = 128, int minimum = 16)
        {
            _maximum = maximum;
            _minimum = minimum;
        }

        public void Enqueue(Action act)
        {
            if (Interlocked.Read(ref _numFree) > 0 || Interlocked.Read(ref _numTotal) >= _maximum)
            {
                _queue.Enqueue(act);
                _event.Set();
                return;
            }
            Interlocked.Increment(ref _numTotal);
            new Thread(() => ThreadProc(act)) {Name = "evhttp pool thread"}.Start();

        }

        void ThreadProc(Action act)
        {
            while (act != null)
            {
                act();

                if (!_queue.TryDequeue(out act))
                {
                    Interlocked.Increment(ref _numFree);

                    bool firstWait = true;
                    while (act == null)
                    {
                        if (!_queue.TryDequeue(out act))
                        {
                            act = null;
                            if (_numTotal > _minimum && !firstWait)
                                break;
                        }

                        var timeout = firstWait ? 100 : -1;
                        _event.WaitOne(timeout);
                        firstWait = false;
                    }
                    Interlocked.Decrement(ref _numFree);
                }
            }
            Interlocked.Decrement(ref _numTotal);
        }
    }

}
