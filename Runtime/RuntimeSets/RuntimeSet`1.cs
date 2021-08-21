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
    using Object = UnityEngine.Object;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Runtime Set", MenuName = Config.PackageName + "Runtime Set")]
    [SuppressMessage("ReSharper", "Unity.NoNullPropagation",
        Justification = "It's ok to invoke scriptable events via null-coalescing operators because the event " +
                        "can be null only if it is not assigned in the inspector.")]
    public class RuntimeSet<TUnityObject> : BaseRuntimeSet, IList<TUnityObject>, IEvent<(int Index, TUnityObject Item)>
#if UNIRX
        , IDisposable
#endif
        where TUnityObject : Object
    {
        private List<TUnityObject> _list = new List<TUnityObject>();

        internal override List<Object> List => _list.ConvertAll(item => (Object) item);

        #region IList

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public TUnityObject this[int index]
        {
            get => _list[index];
            set
            {
                var previousValue = _list[index];
                _list[index] = value;

                if (!ReferenceEquals(previousValue, value))
                    _replaceEvent.NotifyListeners((index, previousValue, value));
            }
        }

        public void Add(TUnityObject item)
        {
            if (_list.Contains(item))
                return;

            _list.Add(item);
            _addEvent.NotifyListeners((_list.Count-1, item));
        }

        public void CopyTo(TUnityObject[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(TUnityObject item)
        {
            int itemIndex = _list.IndexOf(item);

            if (itemIndex == -1)
            {
                return false;
            }

            _list.Remove(item);
            _removeEvent.NotifyListeners((itemIndex, item));
            _countChangeEvent.NotifyListeners(Count);
            return true;
        }

        public void Clear()
        {
            var initialCount = _list.Count;

            for (int i = _list.Count - 1; i >= 0; i--)
            {
                var item = _list[i];
                _list.RemoveAt(i);
                _removeEvent.NotifyListeners((i, item));
            }

            _resetEvent.NotifyListeners();

            if (initialCount > 0)
                _countChangeEvent.NotifyListeners(0);
        }

        public bool Contains(TUnityObject value) => _list.Contains(value);

        public int IndexOf(TUnityObject value) => _list.IndexOf(value);

        public void RemoveAt(int index)
        {
            TUnityObject item = _list[index];
            _list.RemoveAt(index);
            _removeEvent.NotifyListeners((index, item));
        }

        public void Insert(int index, TUnityObject item)
        {
            _list.Insert(index, item);
            _addEvent.NotifyListeners((index, item));
            _countChangeEvent.NotifyListeners(Count);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<TUnityObject> IEnumerable<TUnityObject>.GetEnumerator() => GetEnumerator();

        [PublicAPI]
        public List<TUnityObject>.Enumerator GetEnumerator() => _list.GetEnumerator();

        #endregion

        public override string ToString() => $"Collection<{typeof(TUnityObject)}>({Count})";

        private void OnEnable()
        {
            _countChangeEvent = new EventHelperWithDefaultValue<int>(() => Count);

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

        #region SubEvents

        private readonly EventHelper<(int Index, TUnityObject Item)> _addEvent = new EventHelper<(int Index, TUnityObject Item)>();
        public IEventHelper<(int Index, TUnityObject Item)> AddEvent => _addEvent;
        internal override List<Object> AddListeners => _addEvent.Listeners;

        private EventHelperWithDefaultValue<int> _countChangeEvent;
        public IEventHelperWithDefaultValue<int> CountChangeEvent => _countChangeEvent;
        internal override List<Object> CountChangeListeners => _countChangeEvent?.Listeners;

        private readonly EventHelper<(int OldIndex, int NewIndex, TUnityObject Item)> _moveEvent = new EventHelper<(int OldIndex, int NewIndex, TUnityObject Item)>();
        public IEventHelper<(int OldIndex, int NewIndex, TUnityObject Item)> MoveEvent => _moveEvent;
        internal override List<Object> MoveListeners => _moveEvent.Listeners;

        private readonly EventHelper<(int Index, TUnityObject Item)> _removeEvent = new EventHelper<(int Index, TUnityObject Item)>();
        public IEventHelper<(int Index, TUnityObject Item)> RemoveEvent => _removeEvent;
        internal override List<Object> RemoveListeners => _removeEvent.Listeners;

        private readonly EventHelper<(int Index, TUnityObject OldValue, TUnityObject NewValue)> _replaceEvent = new EventHelper<(int Index, TUnityObject OldValue, TUnityObject NewValue)>();
        public IEventHelper<(int Index, TUnityObject OldValue, TUnityObject NewValue)> ReplaceEvent => _replaceEvent;
        internal override List<Object> ReplaceListeners => _replaceEvent.Listeners;

        private readonly EventHelper _resetEvent = new EventHelper();
        public IEventHelper ResetEvent => _resetEvent;
        internal override List<Object> ResetListeners => _resetEvent.Listeners;

        #endregion

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (newIndex < 0 || newIndex >= _list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            var item = _list[oldIndex];
            _list.RemoveAt(oldIndex);
            _list.Insert(newIndex, item);
            _moveEvent.NotifyListeners((oldIndex, newIndex, item));
        }

        #region UniRx

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _addEvent.Dispose();
            _resetEvent.Dispose();
            _moveEvent.Dispose();
            _removeEvent.Dispose();
            _replaceEvent.Dispose();
            _countChangeEvent.Dispose();
        }

        #endregion
    }
}