using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace XChat2.Common.Collections
{
    //******************************************************************************
    public class SyncQueue<T>
    {
        public SyncQueue() { }

        //--------------------------------------------------------------------------
        private Queue<T> _q = new Queue<T>();

        //==========================================================================
        // <summary>Trägt einen Eintrag (ohne zu warten) ein</summary>
        public void Enqueue(T tItem)
        {
            lock(this)
            {
                _q.Enqueue(tItem);
                Monitor.Pulse(this);
            }
        }

        //==========================================================================
        // <summary>
        //    Holt einen Eintrag aus der Queue heraus und wartet dabei nötigenfalls
        //    solange bis wieder ein Eintrag vorhanden ist.
        // </summary>
        public T Dequeue()
        {
            lock(this)
            {
                while(_q.Count == 0)
                {
                    Monitor.Wait(this);
                }
                return _q.Dequeue();
            }
        }

        public int Count
        {
            get { return _q.Count; }
        }
    }
}