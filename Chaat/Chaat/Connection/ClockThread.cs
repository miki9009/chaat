using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chaat.Connection
{
    public class ClockThread
    {
        public delegate void Method();
        private delegate void MethodLoop();
        private Method method;

        private Thread thread;
        private bool loop;
        private int interval = 0;

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }


        public ClockThread(Method method)
        {
            this.method = method;
        }

        public ClockThread(Method method, bool loop, int interval)
        {
            this.method = method;
            this.loop = loop;
            this.interval = interval;
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Run()
        {
            Thread.Sleep(interval);
            method();
            while (loop)
            {
                Thread.Sleep(interval);
                method();
            }
        }

        public void Stop()
        {
            loop = false;
        }

    }
}
