using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class SynchronizedList<T> : ISynchronizedList, ISynchronized, IList<T>
    {
        public GameObject BaseObject { get; private set; }
        public ushort SynchronizationId { get; private set; }
        private ITypeWriter serializer;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                return items[index];
            }
            set
            {
                if (Engine.IsClient)
                    throw new InvalidOperationException("Only the server may set an index in a SynchronizedList");
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                items[index] = value;
                BaseObject.Send(new SynchronizedListSet(BaseObject, SynchronizationId, (ushort)index, serializer.GetBytes(value)));
            }
        }

        public int Count { get; private set; }
        public int Capacity { get; private set; }
        public bool IsReadOnly { get { return Engine.IsClient; } }

        private T[] items;

        public SynchronizedList()
        {
            serializer = TypeSerializer.GetTypeWriter(GetType().GenericTypeArguments[0]);

            Capacity = 8;
            items = new T[Capacity];
        }

        public void Initialize(GameObject baseObject, ushort synchronizationId)
        {
            this.BaseObject = baseObject;
            this.SynchronizationId = synchronizationId;
        }

        public void Add(T value)
        {
            if (Count + 1 >= Capacity)
            {
                Capacity *= 2;
                Array.Resize(ref items, Capacity);
            }

            BaseObject.Send(new SynchronizedListAdd(BaseObject, SynchronizationId, serializer.GetBytes(value)));
            items[Count] = value;
            Count++;
        }

        public void Insert(int index, T value)
        {
            if (Count + 1 >= Capacity)
            {
                Capacity *= 2;
                Array.Resize(ref items, Capacity);
            }

            BaseObject.Send(new SynchronizedListInsert(BaseObject, SynchronizationId, index, serializer.GetBytes(value)));
            // Copy all items after the current index to the right
            Array.Copy(items, index, items, index + 1, Count - index);
            items[index] = value;

            Count++;
        }

        public bool Remove(T value)
        {
            var index = IndexOf(value);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public int IndexOf(T value)
        {
            if (value == null)
            {
                for (int i = 0; i < Count; i++)
                    if (items[i] == null)
                        return i;
            }
            else
                for (int i = 0; i < Count; i++)
                    if (value.Equals(items[i]))
                        return i;
            return -1;
        }

        public bool Contains(T value)
        {
            return IndexOf(value) != -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index must be greater than or equal to zero and less than SynchronizedList.Count!");
            BaseObject.Send(new SynchronizedListRemoveAt(BaseObject, SynchronizationId, index));

            Array.Copy(items, index + 1, items, index, Count - index);
            Count--;
        }

        public void InsertCommand(object value, int index)
        {
            if (value == null || typeof(T).IsAssignableFrom(value.GetType()))
                throw new InvalidOperationException("Value is not of a valid type!");
            Insert(index, (T)value);
        }
        public void AddCommand(object value)
        {
            if (value == null || typeof(T).IsAssignableFrom(value.GetType()))
                throw new InvalidOperationException("Value is not of a valid type!");
            Add((T)value);
        }
        public void SetCommand(int index, object value)
        {
            if (value == null || typeof(T).IsAssignableFrom(value.GetType()))
                throw new InvalidOperationException("Value is not of a valid type!");
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            items[index] = (T)value;
        }
        public void RemoveCommand(int index)
        {
            RemoveAt(index);
        }

        public void Clear()
        {
            if (Engine.IsClient)
                throw new InvalidOperationException("Only the server may Clear a SynchronizedList!");

            Count = 0;
            // Do not reset capacity, if it ever had a certain number of items it's almost certain it'll get there again
            items = new T[Capacity];
        }

        public void CopyTo(T[] array, int index)
        {
            Array.Copy(items, 0, array, index, Count);
        }

        private IEnumerable<T> Enumerate()
        {
            foreach (var value in items)
                yield return value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Count);
            for (int i=0; i<Count; i++)
                serializer.GenericWrite(items[i], writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            int count = reader.ReadUInt16();
            items = new T[count];
            Capacity = count;

            T[] deserializedItems = new T[count];
            for (int i = 0; i < count; i++)
            {
                Add((T)serializer.GenericRead(reader));
            }
        }
    }
}
