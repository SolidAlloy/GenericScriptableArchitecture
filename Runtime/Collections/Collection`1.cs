namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    [CreateGenericAssetMenu]
    [SuppressMessage("ReSharper", "Unity.NoNullPropagation",
        Justification = "It's ok to invoke scriptable events via null-coalescing operators because the event " +
                        "can be null only if it is not assigned in the inspector.")]
    public class Collection<T> : CollectionBase, IList<T>
    {
        [SerializeField] internal List<T> _list = new List<T>();
        [SerializeField] internal ScriptableEvent<T> _onItemAdded;
        [SerializeField] internal ScriptableEvent<T> _onItemRemoved;

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Add(T item)
        {
            if (_list.Contains(item))
                return;

            _list.Add(item);
            _onItemAdded?.Invoke(item);
        }

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            bool removed = _list.Remove(item);

            if (removed)
            {
                _onItemRemoved?.Invoke(item);
            }

            return removed;
        }

        public void Clear()
        {
            foreach (T item in _list)
            {
                _onItemRemoved?.Invoke(item);
            }

            _list.Clear();
        }

        public bool Contains(T value) => _list.Contains(value);

        public int IndexOf(T value) => _list.IndexOf(value);

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            _onItemRemoved?.Invoke(item);
        }

        public void Insert(int index, T newItem)
        {
            T removedItem = _list[index];
            _onItemRemoved?.Invoke(removedItem);
            _list.Insert(index, newItem);
            _onItemAdded?.Invoke(newItem);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [PublicAPI]
        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

        public override string ToString() => $"Collection<{typeof(T)}>({Count})";

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += CheckList;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += CheckList;
#endif
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= CheckList;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= CheckList;
#endif
        }

        private void CheckList(Scene _)
        {
            CheckList();
        }

#if UNITY_EDITOR
        private void CheckList(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingPlayMode)
                CheckList();
        }
#endif

        private void CheckList()
        {
            foreach (T item in _list)
            {
                if (item == null)
                {
                    Debug.Log("item did not remove itself from the collection");
                    break;
                }
            }
        }
    }
}