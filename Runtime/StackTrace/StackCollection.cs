namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;

    /// <summary>
    /// Stack that implements the <see cref="ICollection{T}"/> interface so that Clear() can be used on it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StackCollection<T> : Stack<T>, ICollection<T>
    {
        public void Add(T item) => Push(item);

        public bool Remove(T item)
        {
            if (item == null || ! item.Equals(Peek()))
            {
                return false;
            }

            Pop();
            return true;
        }

        public bool IsReadOnly => false;
    }
}