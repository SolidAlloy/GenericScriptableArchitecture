namespace GenericScriptableArchitecture
{
    using System.Collections;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;

    [CreateGenericAssetMenu]
    public class Collection<T> : GenericScriptableObject, IList<T>
    {
        [SerializeField] private List<T> _list = new List<T>();

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Add(T obj)
        {
            if ( ! _list.Contains(obj))
                _list.Add(obj);
        }

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(T item) => _list.Remove(item);

        public void Clear() => _list.Clear();

        public bool Contains(T value) => _list.Contains(value);

        public int IndexOf(T value) => _list.IndexOf(value);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public void Insert(int index, T value) => _list.Insert(index, value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [PublicAPI]
        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

        public override string ToString() => $"Collection<{typeof(T)}>({Count})";
    }
}