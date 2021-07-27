namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;

    using Object = UnityEngine.Object;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>, IEvent<T, T>
    {
        [SerializeField] internal T _previousValue;
        [SerializeField] internal bool ListenersWithHistoryExpanded;

        private List<ScriptableEventListener<T, T>> _listeners = new List<ScriptableEventListener<T, T>>();
        private List<IMultipleEventsListener<T, T>> _multipleEventsListeners = new List<IMultipleEventsListener<T, T>>();
        private List<IEventListener<T, T>> _singleEventListeners = new List<IEventListener<T, T>>();
        private List<Action<T, T>> _responses = new List<Action<T, T>>();

        [PublicAPI]
        public T PreviousValue => _previousValue;

        internal override List<Object> ListenersWithHistory
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listeners)
                .OfType<UnityEngine.Object>()
                .ToList();

        public void AddListener(ScriptableEventListener<T, T> listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener<T, T> listener) => _listeners.Remove(listener);

        public void AddListener(IMultipleEventsListener<T, T> listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener<T, T> listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener<T, T> listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener<T, T> listener) => _singleEventListeners.Remove(listener);

        public void AddResponse(Action<T, T> response) => _responses.Add(response);

        public void RemoveResponse(Action<T, T> response) => _responses.Remove(response);

        protected override void InitializeValues()
        {
            base.InitializeValues();
            _previousValue = _initialValue.DeepCopyInEditor();
        }

        protected override void SetValue(T value)
        {
            _previousValue = _value;
            _value = value;
            AddStackTrace(_previousValue, _value);
            InvokeValueChangedEvents();
        }

        internal override void InvokeValueChangedEvents()
        {
            if ( ! CanBeInvoked())
                return;

            base.InvokeValueChangedEvents();

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(_previousValue, _value);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(_previousValue, _value);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this, _previousValue, _value);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised(_previousValue, _value);
            }
        }

        public override string ToString() => $"VariableWithHistory{{{Value}}}";
    }
}