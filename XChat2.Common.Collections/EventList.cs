using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Collections
{
    public class EventList<T> : IEnumerable<T>
    {
        private List<T> _list;

        public EventList()
        {
            _list = new List<T>();
        }

        public EventList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public void Add(T item)
        {
            _list.Add(item);
            OnElementAdded(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            OnElementAdded(index, item);
        }

        public void Remove(T item)
        {
            int index = _list.IndexOf(item);
            _list.Remove(item);
            OnElementRemoved(index, item);
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            OnElementRemoved(index, item);
        }

        public void Clear()
        {
            _list.Clear();
            OnWholeListChanged();
        }

        public int Count
        {
            get { return _list.Count(); }
        }

        #region events
        private void OnElementAdded(T item)
        {
            if (ElementAdded != null)
                ElementAdded(this, Count - 1, item);
            OnListChanged();
        }
        private void OnElementAdded(int index, T item)
        {
            if (ElementAdded != null)
                ElementAdded(this, index, item);
            OnListChanged();
        }
        public delegate void ElementAddedHandler(EventList<T> sender, int index, T addedElement);
        public event ElementAddedHandler ElementAdded;

        private void OnElementRemoved(int index, T item)
        {
            if (ElementRemoved != null)
                ElementRemoved(this, index, item);
            OnListChanged();
        }
        public delegate void ElementRemovedHandler(EventList<T> sender, int index, T removedElement);
        public event ElementRemovedHandler ElementRemoved;

        private void OnListChanged()
        {
            if (ListChanged != null)
                ListChanged(this);
        }
        public delegate void ListChangedHandler(EventList<T> sender);
        public event ListChangedHandler ListChanged;

        private void OnWholeListChanged()
        {
            if (WholeListChanged != null)
                WholeListChanged(this);
            OnListChanged();
        }
        public event ListChangedHandler WholeListChanged;
        #endregion

        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        #endregion;


    }

    public static class EventListExtensions
    {
        public static EventList<TSource> ToEventList<TSource>(this IEnumerable<TSource> collection)
        {
            return new EventList<TSource>(collection);
        }
    }
}
