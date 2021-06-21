namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>
    {
        [SerializeField] internal T _previousValue;
        [SerializeField] internal bool ListenersWithHistoryExpanded;

        private List<ScriptableEventListener<T, T>> _listenersWithHistory = new List<ScriptableEventListener<T, T>>();

        [PublicAPI]
        public T PreviousValue => _previousValue;

        internal override List<ScriptableEventListenerBase> ListenersWithHistory
            => _listenersWithHistory.ConvertAll(item => (ScriptableEventListenerBase) item);

        public void AddListenerOnChangeWithHistory(ScriptableEventListener<T, T> listener)
            => _listenersWithHistory.Add(listener);

        public void RemoveListenerOnChangeWithHistory(ScriptableEventListener<T, T> listener)
            => _listenersWithHistory.Remove(listener);

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
            if (ApplicationUtil.InEditMode)
                return;

            base.InvokeValueChangedEvents();

            for (int i = _listenersWithHistory.Count - 1; i != -1; i--)
            {
                _listenersWithHistory[i].OnEventRaised(_previousValue, _value);
            }
        }

        public override string ToString() => $"VariableWithHistory{{{Value}}}";
    }
}