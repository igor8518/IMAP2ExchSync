using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IMAP2ExchSync
{
    class ProducerTest
    {
        private IIMAP2ExchSync exchposer = null;
        private Queue<int> _queue;
        private SyncEvents _syncEvents;

        public ProducerTest(Queue<int> queue, SyncEvents syncEvents, IIMAP2ExchSync exchposer)
        {
            this.exchposer = exchposer;
            this._queue = queue;
            this._syncEvents = syncEvents;
        }

        protected void Log(int level, string message)
        {

            //exchposer.Log(level, message);

        }
        // Producer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            Random r = new Random();
            while (!_syncEvents.ExitThreadEvent.WaitOne(0, false))
            {
                lock (((ICollection)_queue).SyncRoot)
                {
                    while (_queue.Count < 20)
                    {
                        _queue.Enqueue(r.Next(0, 100));
                        _syncEvents.NewItemEvent.Set();
                        count++;
                    }
                }
            }
            Log(1,string.Format("Producer thread: produced {0} items",count));
        }

    }
    class ConsumerTest
    {
        private IIMAP2ExchSync exchposer = null;
        private Queue<int> _queue;
        private SyncEvents _syncEvents;

        public ConsumerTest(Queue<int> queue, SyncEvents syncEvents, IIMAP2ExchSync exchposer)
        {
            this.exchposer = exchposer;
            this._queue = queue;
            this._syncEvents = syncEvents;
        }

        protected void Log(int level, string message)
        {

           // exchposer.Log(level, message);

        }
        // Producer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1)
            {
                lock (((ICollection)_queue).SyncRoot)
                {
                    int item = _queue.Dequeue();
                }
                count++;
            }
            Log(1, string.Format("Producer thread: produced {0} items", count));
        }

    }
}
