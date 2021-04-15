namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.UnityEngineInternals;
    using UnityEngine;

    [CreateGenericAssetMenu(FileName = "New Variable With History")]
    [Serializable]
    public class VariableWithHistory<T> : Variable<T>
    {

        [SerializeField] internal T _previousValue;

        [SerializeField] private ScriptableEvent<T, T> _changedWithHistory;

        [PublicAPI]
        public T PreviousValue => _previousValue;

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
            if (_changedWithHistory != null) _changedWithHistory.Invoke(_previousValue, _value);
        }

        public override string ToString() => $"VariableWithHistory{{{Value}}}";
    }
}