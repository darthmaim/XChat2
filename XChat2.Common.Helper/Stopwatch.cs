using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Helper
{
    public class Stopwatch : IDisposable
    {
        System.Diagnostics.Stopwatch _sw;
        private bool _running = false;

        public Stopwatch()
        {
            _running = true;

            _sw = new System.Diagnostics.Stopwatch();
            _sw.Start();
            _sw.Stop();
            _sw.Reset();
            _sw.Start();
        }
        
        public bool IsRunning
        {
            get { return _running; }
            set { _running = value; }
        }

        public TimeSpan Elapsed
        {
            get
            {
                if (IsRunning)
                    throw new Exception("Stopwatch is still running");
                return _sw.Elapsed;
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                if (IsRunning)
                    throw new Exception("Stopwatch is still running");
                return _sw.ElapsedMilliseconds;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                if (IsRunning)
                    throw new Exception("Stopwatch is still running");
                return _sw.ElapsedTicks;
            }
        }


        public void Dispose()
        {
            _sw.Stop();
            _running = false;
        }
    }
}
