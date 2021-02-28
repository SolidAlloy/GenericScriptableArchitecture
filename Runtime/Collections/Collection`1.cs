namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [Serializable]
    [CreateGenericAssetMenu]
    [SuppressMessage("ReSharper", "Unity.NoNullPropagation",
        Justification = "It's ok to invoke scriptable events via null-coalescing operators because the event " +
                        "can be null only if it is not assigned in the inspector.")]
    public class Collection<TUnityObject> : CollectionBase, IList<TUnityObject> where TUnityObject : UnityEngine.Object
    {
        [SerializeField] internal ScriptableEvent<TUnityObject> _onItemAdded;
        [SerializeField] internal ScriptableEvent<TUnityObject> _onItemRemoved;

        private List<TUnityObject> _list = new List<TUnityObject>();

        internal override List<UnityEngine.Object> List => _list.ConvertAll(item => (UnityEngine.Object) item);

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public TUnityObject this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Add(TUnityObject item)
        {
            if (_list.Contains(item))
                return;

            _list.Add(item);
            _onItemAdded?.Invoke(item);
        }

        public void CopyTo(TUnityObject[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(TUnityObject item)
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
            foreach (TUnityObject item in _list)
            {
                _onItemRemoved?.Invoke(item);
            }

            _list.Clear();
        }

        public bool Contains(TUnityObject value) => _list.Contains(value);

        public int IndexOf(TUnityObject value) => _list.IndexOf(value);

        public void RemoveAt(int index)
        {
            TUnityObject item = _list[index];
            _list.RemoveAt(index);
            _onItemRemoved?.Invoke(item);
        }

        public void Insert(int index, TUnityObject newItem)
        {
            TUnityObject removedItem = _list[index];
            _onItemRemoved?.Invoke(removedItem);
            _list.Insert(index, newItem);
            _onItemAdded?.Invoke(newItem);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<TUnityObject> IEnumerable<TUnityObject>.GetEnumerator() => GetEnumerator();

        [PublicAPI]
        public List<TUnityObject>.Enumerator GetEnumerator() => _list.GetEnumerator();

        public override string ToString() => $"Collection<{typeof(TUnityObject)}>({Count})";

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
            int nullItemCounter = 0;
            int listCount = _list.Count;

            for (int i = 0; i < listCount; i++)
            {
                if (_list[i] != null)
                    continue;

                _list.RemoveAt(i);
                nullItemCounter++;
            }

            if (nullItemCounter != 0)
                Debug.LogError($"{nullItemCounter} items did not remove themselves from the '{name}' collection before the scene was unloaded.");
        }
    }
}