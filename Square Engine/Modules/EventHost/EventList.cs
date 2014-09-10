using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class OrderedList<T>
    {
        private List<T> list = new List<T>();
        public int Count { get { return list.Count; } }
        public T this[int index] { get { return list[index]; } }

        public void Add(T item)
        {
            var index = ~list.BinarySearch(item);
            list.Insert(index, item);
        }

        public void Remove(T item)
        {
            list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }
    }
}
