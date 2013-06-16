using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace XChat2.Common.Collections
{
    //******************************************************************************
    public class SyncDictionary<K, T>
    {
        public SyncDictionary() { }

        //--------------------------------------------------------------------------
        private Dictionary<K, T> _d = new Dictionary<K, T>();

        //==========================================================================
        // <summary>Trägt einen Eintrag (ohne zu warten) ein</summary>
        public void Add(K key, T item)
        {
            lock(this)
            {
                _d.Add(key, item);
                Monitor.Pulse(this);
            }
        }

        //==========================================================================
        // <summary>
        //    Holt einen Eintrag aus der Queue heraus und wartet dabei nötigenfalls
        //    solange bis wieder ein Eintrag vorhanden ist.
        // </summary>
        public T Get(K key)
        {
            lock(this)
            {
                while(_d.Count == 0 && !_d.ContainsKey(key))
                {
                    Debug.WriteLine(Thread.CurrentThread.Name + " Wait");
                    Monitor.Wait(this);
                }
                Debug.WriteLine(Thread.CurrentThread.Name + " Dequeue ==> " + (_d.Count - 1));
                T item = _d[key];
                _d.Remove(key);
                return item;
            }
        }
    }
}