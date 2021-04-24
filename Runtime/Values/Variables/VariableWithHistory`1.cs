namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.UnityEngineInternals;
    using UnityEngine;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>
    {
        [SerializeField] internal T _previousValue;

        [SerializeField] private ScriptableEvent<T, T> _changedWithHistory;

        private List<ScriptableEventListener<T, T>> _listeners = new List<ScriptableEventListener<T, T>>();

        [PublicAPI]
        public T PreviousValue => _previousValue;

        internal override List<ScriptableEventListenerBase> ListenersWithHistory
            => _listeners.ConvertAll(item => (ScriptableEventListenerBase) item);

        public void AddListenerOnChangeWithHistory(ScriptableEventListener<T, T> listener) => _listeners.Add(listener);

        public void RemoveListenerOnChangeWithHistory(ScriptableEventListener<T, T> listener) => _listeners.Remove(listener);

        protected override void InitializeValues()
        {
            base.InitializeValues();
            _previousValue = _initialValue.DeepCopy();
        }

        protected override void SetValue(T value)
        {
            _previousValue = _value;
            _value = value;
            InvokeValueChangedEvents();
        }

        internal override void InvokeValueChangedEvents()
        {
            if (ApplicationUtil.InEditMode)
                return;

            base.InvokeValueChangedEvents();
            _changedWithHistory?.Invoke(_previousValue, _value);

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(_previousValue, _value);
            }
        }

        public override string ToString() => $"VariableWithHistory{{{Value}}}";
    }
}